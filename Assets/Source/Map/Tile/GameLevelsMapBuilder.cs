using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Installer;
using UniRx;
using UnityEngine;
using Zenject;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace TilesWalk.Map.Tile
{
	/// <summary>
	/// This class builds the game map, the game map contains all the internal levels developed for the game
	/// </summary>
	public class GameLevelsMapBuilder : TileViewMap<GameLevelsMap, GameMapTile, GameMapTileFactory>
	{
		[Inject] private GameLevelsMap _gameMap;

		/// <summary>
		/// If true this script will build the game game from start
		/// The recommended right now is to have the map already built-in
		/// onto the scene, but for future, the map could be loaded through
		/// asset bundles, this would prove useful, optimizations will be necessary.
		/// </summary>
		[SerializeField] private bool _loadAtStart;

		private IDisposable _onNewInstanceDisposable;

#if UNITY_EDITOR
		/// <summary>
		/// On Editor mode this setups this class for building the game map
		/// use in conjunction with <see cref="BuildFromGameMap"/> to continue
		/// with previous progress
		/// </summary>
		[Button(enabledMode: EButtonEnableMode.Editor)]
		private void SetupForEditorMode()
		{
			_factory = GameObject.FindObjectOfType<GameMapTileFactory>();
			_onNewInstanceDisposable?.Dispose();
			Reset();
		}

		/// <summary>
		/// This method creates a new root within the map
		/// </summary>
		[Button(enabledMode: EButtonEnableMode.Editor)]
		private void CreateRootTile()
		{
			var tile = _factory.NewInstance();
			RegisterTile(tile);

			if (_map.Roots == null) _map.Roots = new List<RootTile>();

			tile.Controller.Tile.Root = true;
			_map.Roots.Add(new RootTile()
			{
				Key = TileToHash[tile],
				Position = tile.transform.position,
				Rotation = tile.transform.eulerAngles
			});
		}

		/// <summary>
		/// Saves the current map build to the game maps asset scriptable object
		/// Careful when using as this replaces the current map.
		/// </summary>
		[Button(enabledMode: EButtonEnableMode.Editor)]
		private void SaveGameMap()
		{
			var maps = AssetDatabase.LoadAssetAtPath("Assets/Resources/GameMapsInstaller.asset",
				typeof(GameMapsInstaller)) as GameMapsInstaller;

			if (maps != null)
			{
				// update level maps
				_map.Levels = TileView
					.Where(x => !string.IsNullOrEmpty(x.Value.LevelId))
					.Select(x => new GameLevelsMap.GameLevelReference()
					{
						Id = x.Value.LevelId,
						Hash = TileToHash[x.Value]
					}).ToList();

				maps.GameMap = _map;
				EditorUtility.SetDirty(maps);
			}
			else
			{
				Debug.LogError("GameMapsInstaller scriptable object couldn't be found at " +
				               "Assets/Resources/GameMapsInstaller.asset");
			}
		}

		/// <summary>
		/// In editor mode built the current game map
		/// </summary>
		[Button(enabledMode: EButtonEnableMode.Editor)]
		private void BuildFromGameMap()
		{
			var maps = AssetDatabase.LoadAssetAtPath("Assets/Resources/GameMapsInstaller.asset",
				typeof(GameMapsInstaller)) as GameMapsInstaller;

			if (maps != null)
			{
				BuildTileMap<GameMapTile>(maps.GameMap);
			}
			else
			{
				Debug.LogError("GameMapsInstaller scriptable object couldn't be found at " +
				               "Assets/Resources/GameMapsInstaller.asset");
			}
		}
#endif

		protected override void Start()
		{
			base.Start();

			if (_loadAtStart)
			{
				BuildTileMap<GameMapTile>(_gameMap);
			}
		}

		/// <summary>
		/// Reset the map completely and destroy all the instances
		/// </summary>
		public override void Reset()
		{
			// reset data structures
			if (TileView.Count > 0)
			{
				foreach (var value in TileView.Values)
				{
					if (value == null) continue;

					if (Application.isEditor)
					{
						DestroyImmediate(value.gameObject);
					}
					else
					{
						Destroy(value.gameObject);
					}
				}
			}

			if (Application.isEditor)
			{
				foreach (Transform child in transform)
				{
					DestroyImmediate(child.gameObject);
				}
			}

			TileView.Clear();
			HashToTile.Clear();
			TileToHash.Clear();
			Insertions.Clear();

			_map = new GameLevelsMap();
		}

		/// <summary>
		/// Removes a tile from the map. Handler re-rooting for when a root
		/// tile is removed
		/// </summary>
		/// <param name="tile"></param>
		public override void RemoveTile(GameMapTile tile)
		{
			if (!TileToHash.TryGetValue(tile, out var hash)) return;

			if (IsLevelTile(tile.Controller.Tile))
			{
				_map.Levels.RemoveAll(x => x.Hash == hash);
			}

			TileToHash.Remove(tile);
			HashToTile.Remove(hash);
			TileView.Remove(tile.Controller.Tile);

			// remove from map
			_map.Instructions.RemoveAll(x => x.Tile == hash);
			_map.Instructions.RemoveAll(x => x.Root == hash);

			if (tile.Controller.Tile.Root)
			{
				var index = _map.Roots.FindIndex(x => x.Key == hash);

				if (index >= 0)
				{
					_map.Roots.RemoveAt(index);

					foreach (var tileNeighbor in tile.Controller.Tile.Neighbors)
					{
						tileNeighbor.Value.Root = true;
						var view = TileView[tileNeighbor.Value];

						_map.Roots.Add(new RootTile()
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

		/// <summary>
		/// This register a <see cref="GameMapTile"/> as a tile pointer to
		/// a level to the internal map structure
		/// </summary>
		/// <param name="tile"></param>
		public void RegisterLevelTile(GameMapTile tile)
		{
			if (TileToHash.TryGetValue(tile, out var hash))
			{
				if (_map.Levels.Any(x => x.Hash == hash))
				{
					Debug.LogWarning($"Level {tile.LevelId} tile already registered");
					return;
				}

				_map.Levels.Add(new GameLevelsMap.GameLevelReference()
				{
					Hash = hash,
					Id = tile.LevelId
				});
			}
		}

		/// <summary>
		/// Determines if a <see cref="Tile"/> is actually a tile that points to
		/// a game level
		/// </summary>
		/// <param name="tile"></param>
		/// <returns></returns>
		public bool IsLevelTile(TilesWalk.Tile.Tile tile)
		{
			foreach (var gameLevelReference in _map.Levels)
			{
				var instance = HashToTile[gameLevelReference.Hash];

				if (instance != null && instance.Controller.Tile == tile)
				{
					if (!string.IsNullOrEmpty(instance.LevelId))
					{
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Does nothing in the game map builder
		/// </summary>
		/// <param name="tile"></param>
		protected override void OnNewTileInstance(GameMapTile tile)
		{
			// do nothing
		}

		/// <summary>
		/// Register a new tile to the map structure
		/// </summary>
		/// <param name="tile"></param>
		/// <param name="hash"></param>
		public override void RegisterTile(GameMapTile tile, int? hash = null)
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
		/// Builds the given game map
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="map"></param>
		public override void BuildTileMap<T>(GameLevelsMap map)
		{
			Reset();
			List<int> currentRoots = new List<int>();
			// dictionary of levels, useful for fast checking level tiles
			Dictionary<int, string> levelTiles = new Dictionary<int, string>();

			foreach (var gameLevelReference in map.Levels)
			{
				levelTiles.Add(gameLevelReference.Hash, gameLevelReference.Id);
			}

			// create root tiles first
			foreach (var rootTile in map.Roots)
			{
				T tile = null;
				tile = _factory.NewInstance<T>();
				RegisterTile(tile, rootTile.Key);
				currentRoots.Add(rootTile.Key);
				// set transform
				tile.transform.position = rootTile.Position;
				tile.transform.rotation = Quaternion.Euler(rootTile.Rotation);
				// register root within the tile map
				if (_map.Roots == null) _map.Roots = new List<RootTile>();

				_map.Roots.Add(rootTile);
				tile.Controller.Tile.Root = true;

				tile.name = $"(Root) {tile.Controller.Tile.Index}";

				// check if root is a level tile
				if (levelTiles.TryGetValue(rootTile.Key, out var levelId))
				{
					tile.name = $"'{levelId}' {tile.name}";
					tile.LevelId = levelId;
					tile.ConvertToLevelTile();
				}
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
						// create neighbor tile
						T tile = null;
						tile = _factory.NewInstance<T>();
						RegisterTile(tile, instruction.Tile);
						// insert on the tile structure
						var insert = HashToTile[instruction.Tile];
						rootTile.InsertNeighbor(instruction.Direction, instruction.Rule, insert);
						// register to internal structure
						UpdateInstructions(rootTile, insert, instruction.Direction, instruction.Rule);
						// update newer roots for next loop
						newRoots.Add(instruction.Tile);

						tile.name = $"{tile.Controller.Tile.Index} -> {rootTile.Controller.Tile.Index}, {instruction.Direction} & {instruction.Rule}";

						// check if root is a level tile
						if (levelTiles.TryGetValue(instruction.Tile, out var levelId))
						{
							tile.name = $"'{levelId}' {tile.name}";
							insert.LevelId = levelId;
							insert.ConvertToLevelTile();
						}
					}
				}

				currentRoots = newRoots;
			}

			_map.Id = map.Id;
			_map.MapSize = map.MapSize;
			_onLevelMapLoaded?.OnNext(_map);
		}
	}
}