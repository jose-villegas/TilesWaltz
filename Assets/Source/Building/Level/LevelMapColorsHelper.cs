using System;
using System.Collections.Generic;
using TilesWalk.Gameplay.Display;
using TilesWalk.Tile;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Building.Level
{
	public class LevelMapColorsHelper : MonoBehaviour
	{
		[Inject] private TileViewLevelMap _levelMap;
		[Inject] private GameTileColorsConfiguration _colorsConfiguration;

#if UNITY_EDITOR
		[Header("Editor")]
		[SerializeField] private bool _drawAll = true;
		[SerializeField] private TileColor _drawColorCircles;
#endif

		private Dictionary<TileColor, List<Tile.Tile>> _tilesPerColor = new Dictionary<TileColor, List<Tile.Tile>>();

		private void Awake()
		{
			_levelMap.OnTileRegisteredAsObservable().Subscribe(OnTileRegistered).AddTo(this);
		}

		private void OnTileRegistered(TileView tileView)
		{
			var color = tileView.Controller.Tile.TileColor;

			if (_tilesPerColor == null) _tilesPerColor = new Dictionary<TileColor, List<Tile.Tile>>();

			if (!_tilesPerColor.TryGetValue(color, out var colorTiles))
			{
				_tilesPerColor[color] = colorTiles = new List<Tile.Tile>();
			}

			colorTiles.Add(tileView.Controller.Tile);

			tileView.Controller.Tile.OnTileColorChangedAsObservable().Subscribe(OnTileColorChanged).AddTo(this);
		}

		private void OnTileColorChanged(Tuple<Tile.Tile, TileColor> change)
		{
			var prevColor = change.Item2;
			var current = change.Item1.TileColor;

			// find tile in the dict to move
			if (!_tilesPerColor.TryGetValue(prevColor, out var colorTiles))
			{
				Debug.LogError("This tile wasn't registered");
				return;
			}

			// remove tile from prev color
			colorTiles.Remove(change.Item1);

			// insert onto new color key
			if (!_tilesPerColor.TryGetValue(current, out colorTiles))
			{
				_tilesPerColor[current] = colorTiles = new List<Tile.Tile>();
			}

			colorTiles.Add(change.Item1);
		}

#if UNITY_EDITOR
		private void OnDrawGizmosSelected()
		{
			var colorsToDraw = _drawAll ? Enum.GetValues(typeof(TileColor)) : new[] {_drawColorCircles};

			foreach (TileColor color in colorsToDraw)
			{
				if (!_tilesPerColor.TryGetValue(color, out var colorTiles))
				{
					continue;
				}

				UnityEditor.Handles.color = _colorsConfiguration[color];

				for (int i = 0; i < colorTiles.Count; i++)
				{
					var tileView = _levelMap.GetTileView(colorTiles[i]);
					var height = tileView.transform.position.y;
					var center = Vector3.zero + Vector3.up * height;
					var radius = Vector3.Distance(center, tileView.transform.position);

					UnityEditor.Handles.DrawWireDisc(center, Vector3.up, radius);
				}
			}
		}
#endif
	}
}