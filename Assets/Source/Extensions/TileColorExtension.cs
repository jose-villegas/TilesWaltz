using System;
using System.Linq;
using TilesWalk.Tile;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TilesWalk.Extensions
{
	public static class TileColorExtension
	{
		public static Color Color(this TileColor color)
		{
			switch (color)
			{
				case TileColor.Red:
					return UnityEngine.Color.red;
				case TileColor.Orange:
					return new Color(1f, 0.65f, 0f);
				case TileColor.Yellow:
					return UnityEngine.Color.yellow;
				case TileColor.Green:
					return UnityEngine.Color.green;
				case TileColor.Blue:
					return UnityEngine.Color.blue;
				case TileColor.Purple:
					return new Color(0.63f, 0.13f, 0.94f);
				case TileColor.None:
					return UnityEngine.Color.white;;
			}

			return UnityEngine.Color.gray;
		}

		public static TileColor RandomColor(params TileColor[] filter)
		{
			var values = Enum.GetValues(typeof(TileColor)).OfType<TileColor>().ToList();
			values = values.Where(x => !filter.Contains(x)).ToList();
			return values.ElementAt(Random.Range(1, values.Count));
		}
	}
}