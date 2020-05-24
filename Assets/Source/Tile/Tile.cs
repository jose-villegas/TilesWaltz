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

		[SerializeField] private Tuple<CardinalDirection, NeighborWalkRule> _insertionRule;

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

				// non-origin tile, check for path behavior
				if (_insertionRule.Item1 != CardinalDirection.None)
				{
					var sourceDirection = _insertionRule.Item1.Opposite();

					if (Neighbors.TryGetValue(sourceDirection, out var neighbor))
					{
						var path = neighbor.InsertionRule.Item2.GetPathBehaviour(_insertionRule.Item2);

						if ((path & (PathBehaviourRule.VerticalContinuous | PathBehaviourRule.HorizontalBreak)) > 0)
						{
							return new Bounds(Position,
								new Vector3(_bounds.size.x, _bounds.size.z, _bounds.size.y));
						}
					}
				}


				return new Bounds(Position, _bounds.size);
			}
			set
			{
				_bounds = value;
				// notify others
				NotifyChange(this, new PropertyChangedEventArgs("Bounds"));
			}
		}

		public Tuple<CardinalDirection, NeighborWalkRule> InsertionRule
		{
			get => _insertionRule;
			set => _insertionRule = value;
		}

		public Tile()
		{
			_color = new Color();
			Neighbors = new Dictionary<CardinalDirection, Tile>();
			_index = Vector3.zero;
			Bounds = new Bounds();
			_model = Matrix4x4.identity;
			// origin
			_insertionRule =
				new Tuple<CardinalDirection, NeighborWalkRule>(CardinalDirection.None, NeighborWalkRule.Plain);
		}
	}
}