using System.Collections.Generic;
using System.Linq;
using TilesWalk.Gameplay.Display;
using TilesWalk.General;
using TilesWalk.Tile.Rules;

namespace TilesWalk.Extensions
{
	/// <summary>
	/// Extension methods for the <see cref="Tile.Tile"/> class
	/// </summary>
	public static class TileExtension
	{
		/// <summary>
		/// This method determines if it's possible to insert a tile as neighbor
		/// </summary>
		/// <param name="source"></param>
		/// <param name="direction"></param>
		/// <param name="rule"></param>
		/// <returns></returns>
		public static bool IsValidInsertion(this Tile.Tile source, CardinalDirection direction, NeighborWalkRule rule) 
		{
			bool result = false;

			// first check if direction is already occupied
			if (source.Neighbors.ContainsKey(direction))
			{
				return source.Neighbors[direction] == null;
			}

			return true;
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

        public static CardinalDirection GetNeighborDirection(this Tile.Tile tile, Tile.Tile neighbor)
        {
            foreach (var tileNeighbor in tile.Neighbors)
            {
                if (tileNeighbor.Value == neighbor) return tileNeighbor.Key;
            }

            return CardinalDirection.None;
        }

        public static List<Tile.Tile> GetStraightPath(this Tile.Tile tile, params CardinalDirection[] direction)
        {
            var result = new List<Tile.Tile>() { tile };
            var currentTile = tile;

            foreach (var cardinalDirection in direction)
            {
                currentTile = tile;

                while (currentTile.Neighbors.TryGetValue(cardinalDirection, out var neighbor))
                {
                    currentTile = neighbor;
                    result.Add(currentTile);
                }
            }

            result.Sort((t1, t2) =>
            {
                var dst1 = (tile.Index - t1.Index).sqrMagnitude;
                var dst2 = (tile.Index - t2.Index).sqrMagnitude;
                return dst1 - dst2;
            });

            return result;
        }

        public static List<Tile.Tile> GetAllOfColor(this IEnumerable<Tile.Tile> tiles, TileColor color)
        {
			var result = new List<Tile.Tile>();

            foreach (var tile in tiles)
            {
                if (tile.TileColor == color) result.Add(tile);
            }

            return result;
        }

        public static void ChainRefreshPaths(this Tile.Tile source, CardinalDirection ignore = CardinalDirection.None,
            bool updateColorPath = true, bool updateShortestPath = true)
        {
            if (updateColorPath) source.RefreshMatchingColorPatch();
            if (updateShortestPath) source.RefreshShortestLeafPath();

            foreach (var neighbor in source.Neighbors)
            {
                if (neighbor.Key == ignore) continue;

                ChainRefreshPaths(neighbor.Value, neighbor.Key.Opposite(), updateColorPath, updateShortestPath);
            }
        }
	}
}