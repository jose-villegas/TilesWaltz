using System;
using System.Collections.Generic;
using System.Linq;
using TilesWalk.Gameplay;
using TilesWalk.General;
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

		/// <summary>
		/// Determines if this tile has only one neighbor, meaning
		/// it's a leaf as no other tiles comes from it
		/// </summary>
		/// <param name="tile"></param>
		/// <returns></returns>
		public static bool IsLeaf(this Tile.Tile tile)
		{
			return tile.Neighbors.Count == 1;
		}

		/// <summary>
		/// This method finds the shortest path possible following all the paths
		/// available by walking through the neighbors
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static List<Tile.Tile> GetShortestLeafPath(this Tile.Tile source,
			CardinalDirection direction = CardinalDirection.None)
		{
			List<Tile.Tile> result = new List<Tile.Tile>();
			var keys = source.Neighbors.Keys;

			var count = int.MaxValue;
			foreach (var key in keys)
			{
				// avoid infinite loop
				if (direction != CardinalDirection.None && key == direction.Opposite()) continue;

				var value = source.Neighbors[key];

				if (value == null) continue;

				var trace = GetShortestLeafPath(value, key);

				if (trace != null && trace.Count < count)
				{
					result = trace;
					// update minimum
					count = trace.Count;
				}
			}

			result.Add(source);
			return result;
		}
	}
}