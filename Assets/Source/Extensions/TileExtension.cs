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
		/// Determines if this tile has only one neighbor of the same color, meaning
		/// it's a "color leaf" as no other tiles with the same color comes from it
		/// </summary>
		/// <param name="tile"></param>
		/// <returns></returns>
		public static bool IsColorLeaf(this Tile.Tile tile)
		{
			return tile.Neighbors.Count(x => x.Value.TileColor == tile.TileColor) <= 1;
		}

		/// <summary>
		/// This method finds the shortest path possible following all the paths
		/// available by walking through the neighbors
		/// </summary>
		/// <param name="source">The root tile</param>
		/// <param name="ignore">
		/// Initial direction to ignore, this parameter is
		/// then used recursively to avoid infinite loops
		/// </param>
		/// <returns></returns>
		public static List<Tile.Tile> GetShortestLeafPath(this Tile.Tile source, params CardinalDirection[] ignore)
		{
			List<Tile.Tile> result = new List<Tile.Tile>();
			var keys = source.Neighbors.Keys;

			var count = int.MaxValue;
			foreach (var key in keys)
			{
				var value = source.Neighbors[key];

				if (value == null) continue;

				// avoid infinite loop
				if (ignore != null && ignore.Length > 0 && ignore.Contains(key)) continue;

				var trace = GetShortestLeafPath(value, key.Opposite());

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

		/// <summary>
		/// This method finds a patch containing all the neighboring
		/// color matching tiles
		/// </summary>
		/// <param name="source">The root tile</param>
		/// <param name="ignore">
		/// Initial direction to ignore, this parameter is
		/// then used recursively to avoid infinite loops
		/// </param>
		/// <returns></returns>
		public static List<Tile.Tile> GetColorMatchPatch(this Tile.Tile source, params CardinalDirection[] ignore)
		{
			List<Tile.Tile> result = new List<Tile.Tile>();
			var keys = source.Neighbors.Keys;

			foreach (var key in keys)
			{
				var value = source.Neighbors[key];

				if (value == null) continue;

				// avoid infinite loop
				if (ignore != null && ignore.Length > 0 && ignore.Contains(key)) continue;

				if (value.TileColor != source.TileColor) continue;

				var trace = GetColorMatchPatch(value, key.Opposite());

				if (trace != null)
				{
					result.AddRange(trace);
				}
			}

			result.Add(source);
			return result;
		}
	}
}