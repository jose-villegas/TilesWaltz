using TilesWalk.General;
using UnityEngine;
using UnityEditor;

public class MapBuilder : EditorWindow
{
	private GameObject _asset;
	private CardinalDirection _insertDirection;

	// Add menu named "My Window" to the Window menu
	[MenuItem("Tools/Map Builder")]
	static void Init()
	{
		// Get existing open window or if none, make a new one:
		MapBuilder window = (MapBuilder) EditorWindow.GetWindow(typeof(MapBuilder));
		window.Show();
	}

	void OnGUI()
	{
		_asset = (GameObject) EditorGUILayout.ObjectField("Asset", _asset, typeof(GameObject), true);
		_insertDirection = (CardinalDirection) EditorGUILayout.EnumPopup("Insertion Direction", _insertDirection);
	}
}