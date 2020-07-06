using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ModestTree;
using NaughtyAttributes;
using Newtonsoft.Json;
using TilesWalk.Building.LevelEditor;
using TilesWalk.Gameplay.Condition;
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
	/// <summary>
	/// This class holds the currently running level map and all its <see cref="TileView"/>
	/// instances
	/// </summary>
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

		/// <summary>
		/// This property handles the current map state, useful for locking movement
		/// or seeing what is actually happening with the tiles
		/// </summary>
		public TileViewLevelMapState State
		{
			get => _state;
			set
			{
				_state = value;
				_onMapStateChanged?.OnNext(_state);
			}
		}

		public LevelMap LevelMap => _levelMap;
		public LevelLoadOptions LoadOption => _loadOption;

		protected Subject<LevelMap> _onLevelMapLoaded;
		protected Subject<TileView> _onTileRegistered;
		protected Subject<TileViewLevelMapState> _onMapStateChanged;

		private TileViewLevelMapState _state;

		/// <summary>
		/// Checks if any of the current states for the map is locking movement
		/// </summary>
		/// <returns></returns>
		public bool IsMovementLocked()
		{
			return State == TileViewLevelMapState.RemovingTile ||
			       State == TileViewLevelMapState.OnComboRemoval ||
			       State == TileViewLevelMapState.OnPowerUpRemoval ||
			       State == TileViewLevelMapState.EditorMode ||
			       State == TileViewLevelMapState.Locked;
		}

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

		/// <summary>
		/// This creates a single tile map, with a 'root' tile
		/// </summary>
		private void BuildEditorRootMap()
		{
			var customLevel = new LevelMap
			{
				Id = Constants.CustomLevelName,
				Instructions = new List<InsertionInstruction>(),
				Roots = new List<RootTile>()
				{
					new RootTile()
				},
				MapSize = 3,
				StarsRequired = 0,
				Target = 0,
				FinishCondition = FinishCondition.MovesLimit,
			};
			// add the root tile
			BuildTileMap<LevelEditorTileView>(customLevel);
		}

		/// <summary>
		/// Get tile callbacks and generalize them for the whole map,
		/// the callbacks are subscribed to the tile, so when they are disposed
		/// with the tile instance
		/// </summary>
		/// <param name="tile"></param>
		private void OnNewTileInstance(TileView tile)
		{
			tile.OnComboRemovalAsObservable()
				.Subscribe(path => _onComboRemoval?.OnNext(path)).AddTo(tile);
			tile.OnTileRemovedAsObservable()
				.Subscribe(path => _onTileRemoved?.OnNext(path)).AddTo(tile);
			tile.OnTileClickedAsObservable()
				.Subscribe(val => _onTileClicked?.OnNext(val)).AddTo(tile);
			tile.OnPowerUpRemovalAsObservable()
				.Subscribe(path => _onPowerUpRemoval?.OnNext(path)).AddTo(tile);
		}

		/// <summary>
		/// This method finally registers a <see cref="TileView"/> to this the internal
		/// map structure, use this to confirm that a tile instance is part of the map
		/// </summary>
		/// <param name="tile"></param>
		/// <param name="hash"></param>
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
			// trigger event
			_onTileRegistered?.OnNext(tile);
		}

		/// <summary>
		/// This method check if any color matching combo existing within all the tiles
		/// This is useful for preventing level finish from getting the last combo points
		/// </summary>
		/// <returns></returns>
		public bool IsAnyComboLeft()
		{
			foreach (var value in TileView.Values)
			{
				if (value.Controller.Tile.MatchingColorPatch.Count > 2)
				{
					return true;
				}
			}

			return false;
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

			if (tile.Controller.Tile.Root)
			{
				var index = _levelMap.Roots.FindIndex(x => x.Key == hash);

				if (index >= 0)
				{
					_levelMap.Roots.RemoveAt(index);

					foreach (var tileNeighbor in tile.Controller.Tile.Neighbors)
					{
						tileNeighbor.Value.Root = true;
						var view = TileView[tileNeighbor.Value];

						_levelMap.Roots.Add(new RootTile()
						{
							Key = TileToHash[view],
							Position = view.transform.position,
							Rotation = view.transform.eulerAngles
						});
					}
				}
				else
				{
					Debug.LogWarning("Root tile is marked as root but wasn't found of the level map" +
					                 "check your level integrity.");
				}
			}

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
				Roots = HashToTile
					.Where(x => x.Value.Controller.Tile.Root)
					.Select(val => new RootTile()
					{
						Key = val.Key,
						Position = val.Value.transform.position,
						Rotation = val.Value.transform.eulerAngles
					}).ToList()
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
			List<int> currentRoots = new List<int>();

			// create root tiles first
			foreach (var rootTile in map.Roots)
			{
				T tile = null;
				tile = _viewFactory.NewInstance<T>();
				RegisterTile(tile, rootTile.Key);
				currentRoots.Add(rootTile.Key);
				// set transform
				tile.transform.position = rootTile.Position;
				tile.transform.rotation = Quaternion.Euler(rootTile.Rotation);
				// register root within the tile map
				_levelMap.Roots.Add(rootTile);
			}

			var hasNewRootAvailable = true;

			// Now execute neighbor insertion logic
			while (hasNewRootAvailable)
			{
				var newRoots = new List<int>();
				hasNewRootAvailable = false;

				// now create all the tiles related to these roots
				foreach (var root in currentRoots)
				{
					var related = map.Instructions.Where(x => x.Root == root).ToList();
					var anyRelated = related.Any();
					// ensure next loop since there is at least another tile parent
					hasNewRootAvailable |= anyRelated;

					// no need to iterate if this tile is a leaf
					if (!anyRelated) continue;

					foreach (var instruction in related)
					{
						var rootTile = HashToTile[instruction.Root];
						var insert = HashToTile[instruction.Tile];
						rootTile.InsertNeighbor(instruction.Direction, instruction.Rule, insert);
						UpdateInstructions(rootTile, insert, instruction.Direction, instruction.Rule);
						// update newer roots for next loop
						newRoots.Add(instruction.Tile);
					}
				}

				currentRoots = newRoots;
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

		public IObservable<TileViewLevelMapState> OnMapStateChangedAsObservable()
		{
			return _onMapStateChanged = _onMapStateChanged ?? new Subject<TileViewLevelMapState>();
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			base.RaiseOnCompletedOnDestroy();
			_onLevelMapLoaded?.OnCompleted();
			_onTileRegistered?.OnCompleted();
			_onMapStateChanged?.OnCompleted();
		}
	}
}