using TilesWalk.BaseInterfaces;
using TilesWalk.General;
using UnityEngine;

namespace TilesWalk.Tile
{

	public class TileController : IController
	{
		[SerializeField]
		private Tile _tile;

		public Tile Tile { get => _tile; }

		public TileController()
		{
			_tile = new Tile();
		}

		public void AddNeighbor(CardinalDirection direction, Tile tile)
		{
			var dirIndex = (int)direction;

			if (dirIndex < 0 || dirIndex > _tile.Neighbors.Length)
			{
				Debug.LogError("Given index is invalid for the tile neighbor array");
				return;
			}

			_tile.Neighbors[dirIndex] = tile;

			// adjust position according to neighbor
			var source = _tile.Position;
			var translate = Vector3.zero;

			switch (direction)
			{
				case CardinalDirection.North:
					translate = Vector3.forward;
					break;
				case CardinalDirection.South:
					translate = Vector3.back;
					break;
				case CardinalDirection.East:
					translate = Vector3.right;
					break;
				case CardinalDirection.West:
					translate = Vector3.left;
					break;
				default:
					break;
			}

			tile.Position = source + translate;
		}
	}
}

