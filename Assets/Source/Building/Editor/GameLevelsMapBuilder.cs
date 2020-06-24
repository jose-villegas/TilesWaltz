using System;
using System.Collections.Generic;
using System.Linq;
using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Installer;
using TilesWalk.General;
using TilesWalk.Map.Scaffolding;
using TilesWalk.Map.Tile;
using TilesWalk.Tile;
using TilesWalk.Tile.Rules;
using UnityEditor;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace TilesWalk.Building.Editor
{
	public class GameLevelsMapBuilder : EditorWindow
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
		private GameLevelsMap _currentMap = new GameLevelsMap();

		// Add menu named "My Window" to the Window menu
		[MenuItem("Tools/Map Builder")]
		static void Init()
		{
			// Get existing open window or if none, make a new one:
			GameLevelsMapBuilder window = (GameLevelsMapBuilder) EditorWindow.GetWindow(typeof(GameLevelsMapBuilder));

			window._levelTile =
				AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Scene/LevelTile.prefab",
					typeof(GameObject)) as GameObject;
			window._regularTile =
				AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Scene/Tile.prefab", typeof(GameObject)) as GameObject;
			window._gameMaps =
				AssetDatabase.LoadAssetAtPath("Assets/Resources/GameMapsInstaller.asset", typeof(GameMapsInstaller)) as
					GameMapsInstaller;

			window.Show();
		}

		void OnGUI()
		{
			_levelTile =
				(GameObject) EditorGUILayout.ObjectField("Level Tile", _levelTile, typeof(GameObject), true);
			_insertRegular = EditorGUILayout.Toggle("Insert Regular Tile", _insertRegular);
			_regularTile =
				(GameObject) EditorGUILayout.ObjectField("Regular Tile", _regularTile, typeof(GameObject), true);
			_gameMaps = (GameMapsInstaller) EditorGUILayout.ObjectField("Game Maps", _gameMaps,
				typeof(GameMapsInstaller),
				true);
			_mapName = EditorGUILayout.TextField("Map Name", _mapName);


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

			if (controller == null && Selection.activeGameObject != null && _gameMaps != null)
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

					_gameMaps.LevelsMap = _currentMap;
				}
			}
		}

		private void BuildFromGameMaps()
		{
			var foundMap = _gameMaps.LevelsMap;

			if (foundMap == null || !GUILayout.Button("Build From Game Map")) return;

			_controllers.Clear();
			_indexes.Clear();

			for (int i = 0; i < foundMap.Tiles.Count; i++)
			{
				var mapTile = foundMap.Tiles[i];
				var isRegular = true;

				if (foundMap.TileParameters != null && foundMap.TileParameters.Count > i)
				{
					isRegular = string.IsNullOrEmpty(foundMap.TileParameters[i]);
				}

				var instance = PrefabUtility.InstantiatePrefab(isRegular ? _regularTile : _levelTile,
					Selection.activeTransform) as GameObject;

				Debug.Assert(instance != null, nameof(instance) + " != null");
				// register the new tile
				_controllers.Add(instance, new TileController());
				_indexes.Add(mapTile, instance);
				_currentMap.Tiles.Add(mapTile);

				if (foundMap.TileParameters != null)
				{
					_currentMap.TileParameters.Add(foundMap.TileParameters[i]);

					// adjust bounds
					var boxCollider = instance.GetComponentInChildren<BoxCollider>();
					_controllers[instance].AdjustBounds(boxCollider.bounds);

					if (isRegular) continue;

					var levelName = foundMap.TileParameters[i];
					var details = instance.GetComponentInChildren<LevelNameRequestHandler>();
					details.RawName = levelName;
				}
			}

			LevelTile tileHandler = null;

			foreach (var instruction in foundMap.Instructions)
			{
				var rootIndex = instruction.Root;
				var tileIndex = instruction.Tile;

				var rootController = _controllers[_indexes[rootIndex]];
				var tileController = _controllers[_indexes[tileIndex]];
				var rootTransform = _indexes[rootIndex].transform;
				var tileTransform = _indexes[tileIndex].transform;

				Debug.Assert(foundMap.TileParameters != null, nameof(foundMap.TileParameters) + " != null");
				
				// check if the root is a regular tile
				var indexOfRoot = foundMap.Tiles.IndexOf(rootIndex);

				if (indexOfRoot > 0 && !string.IsNullOrEmpty(foundMap.TileParameters[indexOfRoot]))
				{
					tileHandler = _indexes[rootIndex].GetComponent<LevelTile>();
				}

				if (tileHandler != null)
				{
					LevelTileLink link = tileHandler.GetLink(_indexes[tileIndex], instruction.Direction);
					var indexOfTile = foundMap.Tiles.IndexOf(tileIndex);

					if (indexOfTile > 0 && !string.IsNullOrEmpty(foundMap.TileParameters[indexOfTile]))
					{
						var neighbor = _indexes[rootIndex].GetComponent<LevelTile>();
						link.Level = neighbor;
					}
					else
					{
						link.Path.Add(_indexes[tileIndex]);
					}
				}

				// adjust neighbor insertion
				rootController.AddNeighbor(instruction.Direction, instruction.Rule, tileController.Tile,
					rootTransform.localToWorldMatrix, out var translate, out var rotate);

				tileTransform.rotation = rootTransform.rotation;
				tileTransform.Rotate(rotate.eulerAngles, Space.World);
				tileTransform.position = rootTransform.position + translate;

				// join hinge points
				var src = rootController.Tile.HingePoints[instruction.Direction];
				var dst = tileController.Tile.HingePoints[instruction.Direction.Opposite()];
				src = rootTransform.position + rootTransform.rotation * src;
				dst = tileTransform.position + tileTransform.rotation * dst;
				tileTransform.position += src - dst;

				_currentMap.Instructions.Add(new InsertionInstruction()
				{
					Direction = instruction.Direction,
					Root = rootIndex,
					Rule = instruction.Rule,
					Tile = tileIndex
				});
			}
		}

		private void NewInstance()
		{
			if (GUILayout.Button("Create Instance"))
			{
				var instance = PrefabUtility.InstantiatePrefab(_insertRegular ? _regularTile : _levelTile,
					Selection.activeTransform) as GameObject;

				Debug.Assert(instance != null, nameof(instance) + " != null");

				var id = Mathf.Abs(instance.GetInstanceID());
				_controllers.Add(instance, new TileController());
				_indexes.Add(id, instance);
				_currentMap.Tiles.Add(id);
				_currentMap.TileParameters.Add(_insertRegular ? string.Empty : _levelName);

				// adjust bounds
				var boxCollider = instance.GetComponentInChildren<BoxCollider>();
				_controllers[instance].AdjustBounds(boxCollider.bounds);

				if (!_insertRegular)
				{
					var details = instance.GetComponentInChildren<LevelNameRequestHandler>();
					details.RawName = _levelName;
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
				_currentMap.TileParameters.Add(_insertRegular ? string.Empty : _levelName);

				if (!_insertRegular)
				{
					var details = instance.GetComponentInChildren<LevelNameRequestHandler>();
					details.RawName = _levelName;
				}

				// adjust bounds
				var boxCollider = instance.GetComponentInChildren<BoxCollider>();
				_controllers[instance].AdjustBounds(boxCollider.bounds);

				// adjust neighbor insertion
				controller.AddNeighbor(_insertDirection, _insertRule, _controllers[instance].Tile,
					Selection.activeTransform.localToWorldMatrix, out var translate, out var rotate);

				instance.transform.rotation = Selection.activeTransform.rotation;
				instance.transform.Rotate(rotate.eulerAngles, Space.World);
				instance.transform.position = Selection.activeTransform.position + translate;

				// join hinge points
				var src = controller.Tile.HingePoints[_insertDirection];
				var dst = _controllers[instance].Tile.HingePoints[_insertDirection.Opposite()];
				src = Selection.activeTransform.position + Selection.activeTransform.rotation * src;
				dst = instance.transform.position + instance.transform.rotation * dst;
				instance.transform.position += src - dst;

				var rootIndex = _indexes.FirstOrDefault(x => x.Value == Selection.activeGameObject);

				_currentMap.Instructions.Add(new InsertionInstruction()
				{
					Direction = _insertDirection,
					Root = rootIndex.Key,
					Rule = _insertRule,
					Tile = id
				});
			}
		}
	}
}