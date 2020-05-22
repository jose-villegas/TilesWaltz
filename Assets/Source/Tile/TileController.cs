using System;
using TilesWalk.BaseInterfaces;
using TilesWalk.Extensions;
using TilesWalk.General;
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
			var source = _tile.Index;
			var translate = _tile.Forward;

			switch (direction)
			{
				case CardinalDirection.North:
					translate = _tile.Forward;
					break;
				case CardinalDirection.South:
					translate = -tile.Forward;
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

			// set orientation 
			switch (rule)
			{
				case NeighborWalkRule.Up:
					tile.Orientation = TileOrientation.Vertical;
					break;
				case NeighborWalkRule.Plain:
					// do nothing
					break;
				case NeighborWalkRule.Down:
					break;
				default:
					tile.Orientation = TileOrientation.Vertical;
					break;
			}

			// connect neighbor references
			_tile.Neighbors[dirIndex] = tile;
			tile.Neighbors[oppositeIndex] = _tile;
		}

		internal void AdjustBounds(Bounds bounds)
		{
			_tile.Bounds = bounds;
		}
	}
}

