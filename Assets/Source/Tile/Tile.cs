using System;
using System.Collections.Generic;
using System.ComponentModel;
using TilesWalk.BaseInterfaces;
using TilesWalk.Extensions;
using TilesWalk.General;
using TilesWalk.Tile.Rules;
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
		private Vector3 _index;

		[SerializeField]
		private Vector3 _position;

		[SerializeField]
		private Bounds[] _bounds;

		[SerializeField]
		private NeighborWalkRule _rule;

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
		public Vector3 Index
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
				switch (_rule)
				{
					case NeighborWalkRule.Plain:
						return _bounds[0];
					case NeighborWalkRule.Down:
					case NeighborWalkRule.Up:
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
		public NeighborWalkRule Rule
		{
			get
			{
				return _rule;
			}

			set
			{
				_rule = value;
				// notify others
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Rule"));
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
			_index = Vector3.zero;
			_bounds = new Bounds[2];
			_rule = NeighborWalkRule.Plain;
		}
	}
}

