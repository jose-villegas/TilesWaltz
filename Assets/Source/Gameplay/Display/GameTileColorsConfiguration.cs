using System;
using System.Collections.Generic;
using System.Linq;
using TilesWalk.Map.Tile;
using UnityEngine;

namespace TilesWalk.Gameplay.Display
{
	[Serializable]
	public class GameTileColorsConfiguration
	{
		[SerializeField] private List<GameColorMatch> _configuration;
		
		public Color this[TileColor color]
		{
			get
			{
				GameColorMatch first = null;

				foreach (var x in _configuration)
				{
					if (x.Tile != color) continue;
					
					first = x;
					break;
				}

				if (first != null) return first.Color;

				return Color.black;
			}
		}

		public Color this[GameColor color]
		{
			get
			{
				GameColorMatch first = null;

				foreach (var x in _configuration)
				{
					if (x.Name != color) continue;

					first = x;
					break;
				}

				if (first != null) return first.Color;

				return Color.black;
			}

		}

		public Color this[LevelMapState state]
		{
			get
			{
				GameColorMatch first = null;

				foreach (var x in _configuration)
				{
					if (x.LevelMapState != state) continue;

					first = x;
					break;
				}

				if (first != null) return first.Color;

				return Color.black;
			}

		}
	}
}