using System;
using System.Collections.Generic;
using System.ComponentModel;
using TilesWalk.BaseInterfaces;
using TilesWalk.Extensions;
using TilesWalk.General;
using UnityEngine;

namespace TilesWalk.Tile
{
	/// <summary>
	/// Tile class, contains all properties and fields related to the in-game tiles
	/// puzzle figure, most property names are self explanatory
	/// </summary>
	[Serializable]
	public class Tile : IModel, INotifyPropertyChanged
	{
		[SerializeField]
		private Vector3Int _index;

		[SerializeField]
		private Vector3 _position;

		[SerializeField]
		private Vector3Int _forward;

		[SerializeField]
		private Bounds[] _bounds;

		[SerializeField]
		private TileOrientation _orientation;

		[SerializeField]
		private Color _color;

		private Dictionary<CardinalDirection, Tile> _neighbors;

		/// <summary>
		/// This is used to notify the model-view to be updated in the game frame, 
		/// its helpful to avoid checking on <see cref="MonoBehaviour.Update"/>
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// This structure contains a reference to the nighbor tiles, useful for indexing
		/// the structure, each index represents an index at <see cref="CardinalDirection"/>
		/// </summary>
		public Dictionary<CardinalDirection, Tile> Neighbors
		{
			get => _neighbors; set => _neighbors = value;
		}

		/// <summary>
		/// This vector contains a 3D coordinate respective to the tile structure, though visually
		/// it doesn't look like a series of voxels, this coordinate represents its position in voxel
		/// space
		/// </summary>
		public Vector3Int Index
		{
			get
			{
				return _index;
			}

			set
			{
				_index = value;
				// notify others
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Index"));
			}
		}
		public Bounds Bounds
		{
			get
			{
				switch (_orientation)
				{
					case TileOrientation.Horizontal:
						return _bounds[0];
					case TileOrientation.Vertical:
						return _bounds[1];
					default:
						break;
				}
				return _bounds[0];
			}
			set
			{
				_bounds[0] = value;
				_bounds[1] = value;
				_bounds[1].extents = new Vector3(_bounds[0].extents.x,
												 _bounds[0].extents.z,
												 _bounds[0].extents.y);

				// notify others
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Bounds"));
			}
		}
		public Vector3Int Forward { get => _forward; set => _forward = value; }
		public TileOrientation Orientation
		{
			get
			{
				return _orientation;
			}

			set
			{
				_orientation = value;

				switch (_orientation)
				{
					case TileOrientation.Horizontal:
						_forward = Vector3IntExtension.forward();
						break;
					case TileOrientation.Vertical:
						_forward = Vector3Int.up;
						break;
					default:
						break;
				}

				// notify others
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Orientation"));
			}
		}

		public Vector3 Position
		{
			get
			{
				return _position;
			}
			set
			{
				_position = value;
				// change boundaries center as well
				_bounds[0].center = _position;
				_bounds[1].center = _position;
				// notify others
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Position"));
			}
		}

		public Tile()
		{
			_color = new Color();
			_neighbors = new Dictionary<CardinalDirection, Tile>();
			_index = Vector3Int.zero;
			_forward = Vector3IntExtension.forward();
			_bounds = new Bounds[2];
		}
	}
}

