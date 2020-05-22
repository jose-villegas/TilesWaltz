using System;
using System.ComponentModel;
using TilesWalk.BaseInterfaces;
using UnityEngine;

namespace TilesWalk.Tile
{
	public class TileView : MonoBehaviour, IView
	{
		[SerializeField]
		private TileController _controller;

		public TileController Controller { get => _controller; }

		public TileView()
		{
			_controller = new TileController();
		}

		private void OnEnable()
		{
			_controller.Tile.PropertyChanged += PropertyChanged;
		}

		private void OnDisable()
		{
			_controller.Tile.PropertyChanged -= PropertyChanged;
		}

		public void PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var tile = _controller.Tile;

			transform.position = Vector3.zero;
			transform.rotation = Quaternion.identity;

			switch (tile.Orientation)
			{
				case TileOrientation.Horizontal:
					transform.rotation = Quaternion.identity;
					break;
				case TileOrientation.Vertical:
					transform.RotateAround(Vector3.zero, Vector3.left, 90);
					break;
				default:
					break;
			}

			transform.position = tile.Index;
		}

		private void OnDrawGizmos()
		{
			var bounds = _controller.Tile.Bounds;

			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(bounds.center, _controller.Tile.Bounds.size);

			Gizmos.color = Color.yellow;
			Gizmos.DrawRay(bounds.center, _controller.Tile.Forward);
		}
	}
}

