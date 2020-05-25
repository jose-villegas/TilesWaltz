using System.IO;
using TilesWalk.General;
using TilesWalk.Tile;
using TilesWalk.Tile.Rules;
using UnityEngine;

namespace TilesWalk.Extensions
{
	public static class TileExtension
	{
		public static bool IsValidInsertion(this Tile.Tile source, CardinalDirection direction, NeighborWalkRule rule)
		{
			bool result = false;

			// first check if direction is already occupied
			if (source.Neighbors.ContainsKey(direction))
			{
				return source.Neighbors[direction] == null;
			}
			else
			{
				return true;
			}
			// todo: add rules here
		}
	}
}
