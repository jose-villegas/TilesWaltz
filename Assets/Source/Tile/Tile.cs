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
	public class Tile : SceneModel
	{
		[SerializeField] private Vector3 _index;

		[SerializeField] private Bounds _bounds;

		[SerializeField] private Color _color;

		/// <summary>
		/// This structure contains a reference to the neighbor tiles, useful for indexing
		/// the structure, each index represents an index at <see cref="CardinalDirection"/>
		/// </summary>
		public Dictionary<CardinalDirection, Tile> Neighbors { get; set; }

		/// <summary>
		/// This vector contains a 3D coordinate respective to the tile structure, though visually
		/// it doesn't look like a series of voxels, this coordinate represents its position in voxel
		/// space
		/// </summary>
		public Vector3 Index
		{
			get => _index;
			set
			{
				_index = value;
				// notify others
				NotifyChange(this, new PropertyChangedEventArgs("Index"));
			}
		}

		public Bounds Bounds
		{
			get
			{
				_bounds.center = Position;
				return _bounds;
			}
			set
			{
				_bounds = value;
				// notify others
				NotifyChange(this, new PropertyChangedEventArgs("Bounds"));
			}
		}

		public Tile()
		{
			_color = new Color();
			Neighbors = new Dictionary<CardinalDirection, Tile>();
			_index = Vector3.zero;
			Bounds = new Bounds();
			_model = Matrix4x4.identity;
		}
	}
}