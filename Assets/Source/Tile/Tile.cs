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
	public class Tile : IModel
	{
		[SerializeField] private Vector3 _index;

		[SerializeField] private Bounds _bounds;

		[SerializeField] private Color _color;

		[SerializeField] private Tuple<CardinalDirection, NeighborWalkRule> _insertionRule;

		/// <summary>
		/// This structure contains a reference to the neighbor tiles, useful for indexing
		/// the structure, each index represents an index at <see cref="CardinalDirection"/>
		/// </summary>
		public Dictionary<CardinalDirection, Tile> Neighbors { get; set; }

		/// <summary>
		/// This points connects this tile with the neighbor tile, useful for positioning
		/// </summary>
		public Dictionary<CardinalDirection, Vector3> HingePoints { get; set; }

		/// <summary>
		/// This vector contains a 3D coordinate respective to the tile structure, though visually
		/// it doesn't look like a series of voxels, this coordinate represents its position in voxel
		/// space
		/// </summary>
		public Vector3 Index
		{
			get => _index;
			set => _index = value;
		}

		public Bounds Bounds
		{
			get => _bounds;
			set => _bounds = value;
		}

		public Tuple<CardinalDirection, NeighborWalkRule> InsertionRule
		{
			get => _insertionRule;
			set => _insertionRule = value;
		}

		public ReactiveTransform Transform { get; set; }

		public Tile()
		{
			_color = new Color();
			_bounds = new Bounds();
			_index = Vector3.zero;
			HingePoints = new Dictionary<CardinalDirection, Vector3>();
			Neighbors = new Dictionary<CardinalDirection, Tile>();
			Transform = new ReactiveTransform();
			// origin tile
			_insertionRule =
				new Tuple<CardinalDirection, NeighborWalkRule>(CardinalDirection.None, NeighborWalkRule.Plain);
		}
	}
}