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
			transform.position = _controller.Tile.Position;
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(_controller.Tile.Bounds.center, _controller.Tile.Bounds.size);
		}
	}
}

