using System;
using System.ComponentModel;
using TilesWalk.Gameplay;
using UnityEngine;
using Random = System.Random;

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
			}

			return UnityEngine.Color.gray;
		}

		public static TileColor RandomColor()
		{
			var values = Enum.GetValues(typeof(TileColor));
			var random = new Random();
			return (TileColor)values.GetValue(random.Next(values.Length));
		}
	}
}