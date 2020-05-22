using System;
using TilesWalk.BaseInterfaces;
using TilesWalk.Extensions;
using TilesWalk.General;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace TilesWalk.Tile
{
	[Serializable]
	public class TileController : IController
	{
		[SerializeField]
		private Tile _tile;

		public Tile Tile { get => _tile; }

		public TileController()
		{
			_tile = new Tile();
		}

		public void AddNeighbor(CardinalDirection direction, NeighborWalkRule rule, Tile tile)
		{
			var dirIndex = (int)direction;
			var oppositeIndex = (int)direction.Opposite();

			if (dirIndex < 0 || dirIndex > _tile.Neighbors.Length)
			{
				Debug.LogError("Given index is invalid for the tile neighbor array");
				return;
			}


			// adjust 3d index according to neighbor
			AdjustIndex(direction, tile);

			// set orientation 
			tile.Orientation = TileExtension.Orientation(rule);

			// set 3d actual position to match with hinge points
			tile.Position = tile.Index;

			// query translation needed to match points
			var sourceHingePoints = _tile.HingePoints(direction);
			var tileHingePoints = tile.HingePoints(direction.Opposite());
			var move = Vector3.zero;

			if (rule == NeighborWalkRule.Up || rule == NeighborWalkRule.Down)
			{
				// change in path orientation
				if (_tile.Orientation == TileOrientation.Horizontal)
				{
					if (rule == NeighborWalkRule.Up)
					{
						if (direction == CardinalDirection.North)
						{
							tile.Position += sourceHingePoints[2] - tileHingePoints[0];
						}
					}
				}
				// orientation stays the same
				else
				{
					if (direction == CardinalDirection.North)
					{
						tile.Position = _tile.Position + _tile.Forward;
					}
				}
			}
			else
			{
				// change in path orientation
				if (_tile.Orientation == TileOrientation.Vertical)
				{
					if (direction == CardinalDirection.North)
					{
						tile.Position += sourceHingePoints[2] - tileHingePoints[0];
					}
				}
				// orientation stays the same way
				else
				{
					switch (direction)
					{
						case CardinalDirection.North:
							tile.Position = _tile.Position + _tile.Forward;
							break;
						case CardinalDirection.South:
							tile.Position = _tile.Position - _tile.Forward;
							break;
						case CardinalDirection.East:
							tile.Position = _tile.Position + Vector3.right;
							break;
						case CardinalDirection.West:
							tile.Position = _tile.Position + Vector3.left;
							break;
						default:
							break;
					}
				}
			}

			// connect neighbor references
			_tile.Neighbors[dirIndex] = tile;
			tile.Neighbors[oppositeIndex] = _tile;
		}

		private void AdjustIndex(CardinalDirection direction, Tile tile)
		{
			var source = _tile.Index;
			var translate = _tile.Forward;

			switch (direction)
			{
				case CardinalDirection.North:
					translate += Vector3IntExtension.forward();
					break;
				case CardinalDirection.South:
					translate += Vector3IntExtension.backward();
					break;
				case CardinalDirection.East:
					translate += Vector3Int.right;
					break;
				case CardinalDirection.West:
					translate += Vector3Int.left;
					break;
				default:
					break;
			}

			tile.Index = source + translate;
		}

		internal void AdjustBounds(Bounds bounds)
		{
			_tile.Bounds = bounds;
		}
	}
}

