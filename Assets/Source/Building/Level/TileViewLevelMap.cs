using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Newtonsoft.Json;
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
		[SerializeField] private LevelLoadOptions _loadOption;
		[TextArea, SerializeField] private string _instructions;
		[SerializeField] LevelMap _levelMap = new LevelMap();

		[Inject] private TileViewFactory _viewFactory;
		[Inject] private MapLevelBridge _mapLevelBridge;

		private Dictionary<Tile.Tile, TileView> _tileView = new Dictionary<Tile.Tile, TileView>();

		public Dictionary<TileView, int> TileToHash { get; } = new Dictionary<TileView, int>();
		public Dictionary<int, TileView> HashToTile { get; } = new Dictionary<int, TileView>();

		public Dictionary<int, List<InsertionInstruction>> Insertions { get; } =
			new Dictionary<int, List<InsertionInstruction>>();

		public LevelMap LevelMap => _levelMap;

		protected Subject<LevelMap> _onLevelMapLoaded;

		private void Start()
		{
			_viewFactory.OnNewInstanceAsObservable().Subscribe(OnNewTileInstance).AddTo(this);

			if (_loadOption == LevelLoadOptions.FromInstructions)
			{
				BuildFromInstructions();
			}
			else if (_loadOption == LevelLoadOptions.FromLevelBridge)
			{
				BuildTileMap(_mapLevelBridge.SelectedLevel);
			}
		}

		private void OnNewTileInstance(TileView tile)
		{
			RegisterTile(tile);
			tile.OnComboRemovalAsObservable()
				.Subscribe(path => _onComboRemoval?.OnNext(path)).AddTo(this);
			tile.OnTileRemovedAsObservable()
				.Subscribe(path => _onTileRemoved?.OnNext(path)).AddTo(this);
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
			_tileView[tile.Controller.Tile] = tile;
			// register tile to the tile map
			_levelMap.Tiles.Add(id);
		}

		public void RemoveTile(TileView tile)
		{
			if (!TileToHash.TryGetValue(tile, out var hash)) return;

			TileToHash.Remove(tile);
			HashToTile.Remove(hash);
			// remove from map
			_levelMap.Instructions.RemoveAll(x => x.tile == hash);
			_levelMap.Instructions.RemoveAll(x => x.root == hash);
			_levelMap.Tiles.Remove(hash);
			// remove all instructions that refer to this tile

			if (!Insertions.TryGetValue(hash, out var instructions)) return;

			Insertions.Remove(hash);

			foreach (var instruction in instructions)
			{
				Destroy(HashToTile[instruction.tile].gameObject);
			}
		}

		public TileView GetTileView(Tile.Tile tile)
		{
			return _tileView[tile];
		}

		[Button]
		public void RefreshAllPaths()
		{
			foreach (var viewKey in _tileView.Keys)
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
			BuildTileMap(map);
		}

		private void BuildTileMap(LevelMap map)
		{
			// reset data structures
			HashToTile.Clear();
			TileToHash.Clear();
			Insertions.Clear();

			foreach (var mapTile in map.Tiles)
			{
				var tile = _viewFactory.NewInstance();
				// register with the source hash
				RegisterTile(tile, mapTile);
			}

			// Now execute neighbor insertion logic
			foreach (var instruction in map.Instructions)
			{
				var rootTile = HashToTile[instruction.root];
				var insert = HashToTile[instruction.tile];
				// adjust neighbor insertion
				rootTile.Controller.AddNeighbor(instruction.direction, instruction.rule, insert.Controller.Tile,
					rootTile.transform, insert.transform);
				UpdateInstructions(rootTile, insert, instruction.direction, instruction.rule);
			}

			_levelMap.Id = map.Id;
			_levelMap.Target = map.Target;
			_levelMap.FinishCondition = map.FinishCondition;
			_levelMap.MapSize = map.MapSize;
			_onLevelMapLoaded?.OnNext(_levelMap);
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
				tile = tileId,
				root = rootId,
				direction = d,
				rule = r
			});

			_levelMap.Instructions.Add(insertions.Last());
		}

		public IObservable<LevelMap> OnLevelMapLoadedAsObservable()
		{
			return _onLevelMapLoaded = _onLevelMapLoaded ?? new Subject<LevelMap>();
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			base.RaiseOnCompletedOnDestroy();
			_onLevelMapLoaded?.OnCompleted();
		}
	}
}