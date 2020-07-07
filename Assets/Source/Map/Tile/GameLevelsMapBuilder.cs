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
	public class GameLevelsMapBuilder : TileLevelMap<GameLevelsMap, GameMapTile, GameMapTileFactory>
	{
		[Inject] private GameLevelsMap _gameMap;
		private IDisposable _onNewInstanceDisposable;

#if UNITY_EDITOR
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

			_map.Roots.Add(new RootTile()
			{
				Key = TileToHash[tile],
				Position = tile.transform.position,
				Rotation = tile.transform.eulerAngles
			});
		}

		[Button(enabledMode: EButtonEnableMode.Editor)]
		private void SaveGameMap()
		{
			var maps = AssetDatabase.LoadAssetAtPath("Assets/Resources/GameMapsInstaller.asset",
				typeof(GameMapsInstaller)) as GameMapsInstaller;

			if (maps != null)
			{
				maps.GameMap = _map;
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
			BuildTileMap<GameMapTile>(_gameMap);
		}

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

			TileView.Clear();
			HashToTile.Clear();
			TileToHash.Clear();
			Insertions.Clear();

			_map = new GameLevelsMap();
		}

		public override void RemoveTile(GameMapTile tile)
		{
			if (!TileToHash.TryGetValue(tile, out var hash)) return;

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
		/// New tiles will be registered as roots
		/// </summary>
		/// <param name="tile"></param>
		protected override void OnNewTileInstance(GameMapTile tile)
		{
			// do nothing
		}

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

		public override void BuildTileMap<T>(GameLevelsMap map)
		{
			Reset();
			List<int> currentRoots = new List<int>();

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
				_map.Roots.Add(rootTile);
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

			_map.Id = map.Id;
			_map.MapSize = map.MapSize;
			_onLevelMapLoaded?.OnNext(_map);
		}
	}
}