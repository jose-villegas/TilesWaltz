using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Newtonsoft.Json;
using TilesWalk.Building.LevelEditor;
using TilesWalk.Gameplay.Level;
using TilesWalk.General;
using TilesWalk.Map.Bridge;
using TilesWalk.Tile;
using TilesWalk.Tile.Rules;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Building.Level
{
	public class TileViewLevelMap : TileViewTrigger
	{
		[Inject] private TileViewFactory _viewFactory;
		[Inject] private LevelBridge _levelBridge;
		[Inject] private CustomLevelsConfiguration _customLevelsConfiguration;

		[SerializeField] private LevelLoadOptions _loadOption;
		[TextArea, SerializeField] private string _instructions;
		[SerializeField] LevelMap _levelMap = new LevelMap();

		private Dictionary<Tile.Tile, TileView> TileView { get; } = new Dictionary<Tile.Tile, TileView>();
		public Dictionary<TileView, int> TileToHash { get; } = new Dictionary<TileView, int>();
		public Dictionary<int, TileView> HashToTile { get; } = new Dictionary<int, TileView>();

		public Dictionary<int, List<InsertionInstruction>> Insertions { get; } =
			new Dictionary<int, List<InsertionInstruction>>();

		public LevelMap LevelMap => _levelMap;
		public LevelLoadOptions LoadOption => _loadOption;

		protected Subject<LevelMap> _onLevelMapLoaded;
		protected Subject<TileView> _onTileRegistered;

		private void Start()
		{
			_viewFactory.OnNewInstanceAsObservable().Subscribe(OnNewTileInstance).AddTo(this);

			switch (_loadOption)
			{
				case LevelLoadOptions.FromInstructions:
					BuildFromInstructions();
					break;
				case LevelLoadOptions.FromBridgeLevelMode:
					BuildTileMap<TileView>(_levelBridge.Payload.Level);
					break;
				case LevelLoadOptions.FromBridgeEditorMode when _levelBridge.Payload == null:
					BuildEditorRootMap();
					break;
				case LevelLoadOptions.FromBridgeEditorMode:
					BuildTileMap<LevelEditorTileView>(_levelBridge.Payload.Level);
					break;
				case LevelLoadOptions.LevelEditor:
					BuildEditorRootMap();
					break;
			}
		}

		private void BuildEditorRootMap()
		{
			var customLevel = new LevelMap
			{
				Tiles = new List<int> {0},
				Id = Constants.CustomLevelName,
				Instructions = new List<InsertionInstruction>(),
				MapSize = 3,
				StarsRequired = 0,
				Target = 0
			};
			// add the root tile
			BuildTileMap<LevelEditorTileView>(customLevel);
		}

		private void OnNewTileInstance(TileView tile)
		{
			tile.OnComboRemovalAsObservable()
				.Subscribe(path => _onComboRemoval?.OnNext(path)).AddTo(tile);
			tile.OnTileRemovedAsObservable()
				.Subscribe(path => _onTileRemoved?.OnNext(path)).AddTo(tile);
			tile.OnTileClickedAsObservable()
				.Subscribe(val => _onTileClicked?.OnNext(val)).AddTo(tile);
		}

		public void RegisterTile(TileView tile, int? hash = null)
		{
			if (TileToHash.ContainsKey(tile))
			{
				var h = TileToHash[tile];
				TileToHash.Remove(tile);
				HashToTile.Remove(h);
			}

			var id = hash ?? tile.GetHashCode();
			TileToHash[tile] = id;
			HashToTile[id] = tile;
			TileView[tile.Controller.Tile] = tile;
			// register tile to the tile map
			_levelMap.Tiles.Add(id);
			// trigger event
			_onTileRegistered?.OnNext(tile);
		}

		public void RemoveTile(TileView tile)
		{
			if (!TileToHash.TryGetValue(tile, out var hash)) return;

			TileToHash.Remove(tile);
			HashToTile.Remove(hash);
			TileView.Remove(tile.Controller.Tile);
			// remove from map
			_levelMap.Instructions.RemoveAll(x => x.Tile == hash);
			_levelMap.Instructions.RemoveAll(x => x.Root == hash);
			_levelMap.Tiles.Remove(hash);
			// remove all instructions that refer to this tile

			if (!Insertions.TryGetValue(hash, out var instructions)) return;

			Insertions.Remove(hash);
		}

		public TileView GetTileView(Tile.Tile tile)
		{
			return TileView[tile];
		}

		public bool HasTileView(Tile.Tile tile)
		{
			return TileView.ContainsKey(tile);
		}

		[Button]
		public void RefreshAllPaths()
		{
			foreach (var viewKey in TileView.Keys)
			{
				viewKey.RefreshShortestLeafPath();
				viewKey.RefreshMatchingColorPatch();
			}
		}

		[Button]
		public void GenerateInstructions()
		{
			var instr = Insertions.Values.SelectMany(x => x).ToList();
			var hashes = TileToHash.Values.ToList();

			var map = new LevelMap()
			{
				Instructions = instr,
				Tiles = hashes
			};

			_instructions = JsonConvert.SerializeObject(map);
		}

		[Button]
		public void BuildFromInstructions()
		{
			// first instance all the needed tiles
			var map = JsonConvert.DeserializeObject<LevelMap>(_instructions);

			switch (_loadOption)
			{
				case LevelLoadOptions.None:
				case LevelLoadOptions.FromInstructions:
				case LevelLoadOptions.FromBridgeLevelMode:
					BuildTileMap<TileView>(map);
					break;
				case LevelLoadOptions.LevelEditor:
				case LevelLoadOptions.FromBridgeEditorMode:
					BuildTileMap<LevelEditorTileView>(map);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void BuildTileMap<T>(LevelMap map) where T : TileView
		{
			Reset();

			foreach (var mapTile in map.Tiles)
			{
				T tile = null;
				tile = _viewFactory.NewInstance<T>();
				RegisterTile(tile, mapTile);
			}

			// Now execute neighbor insertion logic
			foreach (var instruction in map.Instructions)
			{
				var rootTile = HashToTile[instruction.Root];
				var insert = HashToTile[instruction.Tile];
				// adjust neighbor insertion
				Vector3 translate = Vector3.zero;
				Quaternion rotate = Quaternion.identity;
				rootTile.InsertNeighbor(instruction.Direction, instruction.Rule, insert);
				UpdateInstructions(rootTile, insert, instruction.Direction, instruction.Rule);
			}

			_levelMap.Id = map.Id;
			_levelMap.Target = map.Target;
			_levelMap.FinishCondition = map.FinishCondition;
			_levelMap.MapSize = map.MapSize;
			_onLevelMapLoaded?.OnNext(_levelMap);
		}

		public bool IsBreakingDistance(TileView tile)
		{
			var tiles = HashToTile.Values.ToList();
			var tileBounds = new Bounds
			(
				tile.transform.position,
				tile.Collider.size * _customLevelsConfiguration.TileSeparationBoundsOffset
			);

			return tiles.Any(x =>
			{
				var tightBound = new Bounds
				(
					x.transform.position,
					x.Collider.bounds.size * _customLevelsConfiguration.TileSeparationBoundsOffset
				);
				return tightBound.Intersects(tileBounds);
			});
		}

		public void Reset()
		{
			// reset data structures
			if (TileView.Count > 0)
			{
				foreach (var value in TileView.Values)
				{
					Destroy(value.gameObject);
				}
			}

			TileView.Clear();
			HashToTile.Clear();
			TileToHash.Clear();
			Insertions.Clear();
		}

		public void UpdateInstructions(TileView root, TileView tile, CardinalDirection d, NeighborWalkRule r)
		{
			if (!TileToHash.TryGetValue(root, out var rootId) ||
			    !TileToHash.TryGetValue(tile, out var tileId)) return;

			if (!Insertions.TryGetValue(rootId, out var insertions))
			{
				Insertions[rootId] = insertions = new List<InsertionInstruction>();
			}

			insertions.Add(new InsertionInstruction()
			{
				Tile = tileId,
				Root = rootId,
				Direction = d,
				Rule = r
			});

			_levelMap.Instructions.Add(insertions.Last());
		}

		public IObservable<LevelMap> OnLevelMapLoadedAsObservable()
		{
			return _onLevelMapLoaded = _onLevelMapLoaded ?? new Subject<LevelMap>();
		}

		public IObservable<TileView> OnTileRegisteredAsObservable()
		{
			return _onTileRegistered = _onTileRegistered ?? new Subject<TileView>();
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			base.RaiseOnCompletedOnDestroy();
			_onLevelMapLoaded?.OnCompleted();
			_onTileRegistered?.OnCompleted();
		}
	}
}