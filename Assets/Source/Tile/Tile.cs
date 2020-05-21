using System.ComponentModel;
using TilesWalk.BaseInterfaces;
using UnityEngine;

namespace TilesWalk.Tile
{
	public class Tile : IModel, INotifyPropertyChanged
	{
		[SerializeField]
		private Vector3 _position;

		[SerializeField]
		private Bounds _bounds;

		[SerializeField]
		private TileOrientation _orientation;

		[SerializeField]
		private Color _color;

		[SerializeField]
		private Tile[] _neighbors;

		public event PropertyChangedEventHandler PropertyChanged;

		public Tile[] Neighbors
		{
			get => _neighbors; set => _neighbors = value;
		}
		public Vector3 Position
		{
			get => _position;
			set
			{
				_position = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Position"));
			}
		}
		public Bounds Bounds { get => _bounds; set => _bounds = value; }

		public Tile()
		{
			_color = new Color();
			_neighbors = new Tile[4];
			_position = Vector3.zero;
			_bounds = new Bounds(_position, Vector3.one);
		}
	}
}

