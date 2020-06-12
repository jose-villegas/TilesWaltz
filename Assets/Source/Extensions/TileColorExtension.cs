using System;
using System.Linq;
using TilesWalk.Gameplay.Display;
using TilesWalk.Tile;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TilesWalk.Extensions
{
	public static class TileColorExtension
	{
		public static TileColor RandomColor(params TileColor[] filter)
		{
			var values = Enum.GetValues(typeof(TileColor)).OfType<TileColor>().ToList();
			values = values.Where(x => !filter.Contains(x)).ToList();
			return values.ElementAt(Random.Range(1, values.Count));
		}
	}
}