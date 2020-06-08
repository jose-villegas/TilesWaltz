using System;
using System.Collections.Generic;
using System.Linq;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Installer;
using TilesWalk.General;
using TilesWalk.Navigation.Map;
using TilesWalk.Tile;
using TilesWalk.Tile.Rules;
using UnityEditor;
using UnityEngine;

namespace TilesWalk.Building.Editor
{
	public class MapBuilder : EditorWindow
	{
		private GameObject _levelTile;
		private GameObject _regularTile;
		private GameMapsInstaller _gameMaps;
		private string _mapName = "Game Map";
		private bool _insertRegular = true;
		private string _levelName = "none";

		private CardinalDirection _insertDirection;
		private NeighborWalkRule _insertRule = NeighborWalkRule.Plain;

		private Dictionary<int, GameObject> _indexes = new Dictionary<int, GameObject>();
		private Dictionary<GameObject, TileController> _controllers = new Dictionary<GameObject, TileController>();
		private TileMap _currentMap = new TileMap();

		// Add menu named "My Window" to the Window menu
		[MenuItem("Tools/Map Builder")]
		static void Init()
		{
			// Get existing open window or if none, make a new one:
			MapBuilder window = (MapBuilder) EditorWindow.GetWindow(typeof(MapBuilder));

			window._levelTile =
				AssetDatabase.LoadAssetAtPath("Assets/Prefabs/LevelTile.prefab", typeof(GameObject)) as GameObject;
			window._regularTile =
				AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Tile.prefab", typeof(GameObject)) as GameObject;
			window._gameMaps =
				AssetDatabase.LoadAssetAtPath("Assets/Resources/GameMapsInstaller.asset", typeof(GameMapsInstaller)) as
					GameMapsInstaller;

			window.Show();
		}

		void OnGUI()
		{
			_levelTile = (GameObject) EditorGUILayout.ObjectField("Level Tile", _levelTile, typeof(GameObject), true);
			_regularTile =
				(GameObject) EditorGUILayout.ObjectField("Regular Tile", _regularTile, typeof(GameObject), true);
			_gameMaps = (GameMapsInstaller) EditorGUILayout.ObjectField("Game Maps", _gameMaps,
				typeof(GameMapsInstaller),
				true);
			_mapName = EditorGUILayout.TextField("Map Name", _mapName);
			_insertRegular = EditorGUILayout.Toggle("Insert Regular Tile", _insertRegular);
			TileController controller = null;

			if (!_insertRegular)
			{
				_levelName = EditorGUILayout.TextField("Level Name", _levelName);
			}

			if (Selection.activeGameObject != null &&
			    _controllers.TryGetValue(Selection.activeGameObject, out controller))
			{
				NeighborInsertion(controller);
			}
			else
			{
				NewInstance();
			}

			if (controller == null && Selection.activeGameObject != null && _gameMaps != null &&
			    _gameMaps.AvailableMaps != null && _gameMaps.AvailableMaps.Count > 0)
			{
				BuildFromGameMaps();
			}


			GUILayout.FlexibleSpace();

			SaveToGameMaps();
		}

		private void SaveToGameMaps()
		{
			if (GUILayout.Button("Save to Game Map"))
			{
				if (_gameMaps != null)
				{
					_currentMap.Id = _mapName;

					if (_gameMaps.AvailableMaps == null) return;

					var indexOf = _gameMaps.AvailableMaps.FindIndex(x => x.Id == _mapName);

					if (indexOf >= 0)
					{
						_gameMaps.AvailableMaps[indexOf] = _currentMap;
					}
					else
					{
						_gameMaps.AvailableMaps.Add(_currentMap);
					}
				}
			}
		}

		private void BuildFromGameMaps()
		{
			var foundMap = _gameMaps.AvailableMaps.FirstOrDefault(x => x.Id == _mapName);

			if (foundMap != null && GUILayout.Button("Build From Game Map"))
			{
				_controllers.Clear();
				_indexes.Clear();

				for (int i = 0; i < foundMap.Tiles.Count; i++)
				{
					var mapTile = foundMap.Tiles[i];
					var isRegular = bool.Parse(foundMap.TileParameters[i].Split(',')[0]);

					var instance = PrefabUtility.InstantiatePrefab(isRegular ? _regularTile : _levelTile,
						Selection.activeTransform) as GameObject;
					// register the new tile
					_controllers.Add(instance, new TileController());
					_indexes.Add(mapTile, instance);
					_currentMap.Tiles.Add(mapTile);
					_currentMap.TileParameters.Add(foundMap.TileParameters[i]);

					// adjust bounds
					var boxCollider = instance.GetComponentInChildren<BoxCollider>();
					_controllers[instance].AdjustBounds(boxCollider.bounds);

					if (!isRegular)
					{
						var levelName = foundMap.TileParameters[i].Split(',')[1];
						var details = instance.GetComponentInChildren<LevelNameRequestHandler>();
						details.LevelName = levelName;
					}
				}

				foreach (var instruction in foundMap.Instructions)
				{
					var rootIndex = instruction.root;
					var tileIndex = instruction.tile;

					var rootController = _controllers[_indexes[rootIndex]];
					var tileController = _controllers[_indexes[tileIndex]];
					var rootTransform = _indexes[rootIndex].transform;
					var tileTransform = _indexes[tileIndex].transform;

					// adjust neighbor insertion
					rootController.AddNeighbor(instruction.direction, instruction.rule, tileController.Tile,
						rootTransform, tileTransform);

					_currentMap.Instructions.Add(new InsertionInstruction()
					{
						direction = instruction.direction,
						root = rootIndex,
						rule = instruction.rule,
						tile = tileIndex
					});
				}
			}
		}

		private void NewInstance()
		{
			if (GUILayout.Button("Create Instance"))
			{
				var instance = PrefabUtility.InstantiatePrefab(_insertRegular ? _regularTile : _levelTile,
					Selection.activeTransform) as GameObject;

				var id = Mathf.Abs(instance.GetInstanceID());
				_controllers.Add(instance, new TileController());
				_indexes.Add(id, instance);
				_currentMap.Tiles.Add(id);
				_currentMap.TileParameters.Add(_insertRegular.ToString() + "," +
				                               (_insertRegular ? "none" : _levelName));

				// adjust bounds
				var boxCollider = instance.GetComponentInChildren<BoxCollider>();
				_controllers[instance].AdjustBounds(boxCollider.bounds);

				if (!_insertRegular)
				{
					var details = instance.GetComponentInChildren<LevelNameRequestHandler>();
					details.LevelName = _levelName;
				}
			}
		}

		private void NeighborInsertion(TileController controller)
		{
			_insertDirection =
				(CardinalDirection) EditorGUILayout.EnumPopup("Insertion Direction", _insertDirection);
			_insertRule = (NeighborWalkRule) EditorGUILayout.EnumPopup("Insertion Direction", _insertRule);

			if (GUILayout.Button("Add Neighbor"))
			{
				var instance = PrefabUtility.InstantiatePrefab(_insertRegular ? _regularTile : _levelTile,
					Selection.activeTransform.parent) as GameObject;
				_controllers.Add(instance, new TileController());
				var id = Mathf.Abs(instance.GetInstanceID());
				_indexes.Add(id, instance);
				_currentMap.Tiles.Add(id);
				_currentMap.TileParameters.Add(_insertRegular.ToString() + "," +
				                               (_insertRegular ? "none" : _levelName));

				if (!_insertRegular)
				{
					var details = instance.GetComponentInChildren<LevelNameRequestHandler>();
					details.LevelName = _levelName;
				}

				// adjust bounds
				var boxCollider = instance.GetComponentInChildren<BoxCollider>();
				_controllers[instance].AdjustBounds(boxCollider.bounds);

				controller.AddNeighbor(_insertDirection, _insertRule, _controllers[instance].Tile,
					Selection.activeTransform, instance.transform);

				var rootIndex = _indexes.FirstOrDefault(x => x.Value == Selection.activeGameObject);

				_currentMap.Instructions.Add(new InsertionInstruction()
				{
					direction = _insertDirection,
					root = rootIndex.Key,
					rule = _insertRule,
					tile = id
				});
			}
		}
	}
}