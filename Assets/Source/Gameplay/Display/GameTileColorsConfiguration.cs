using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TilesWalk.Gameplay.Display
{
	[Serializable]
	public class GameTileColorsConfiguration
	{
		[Serializable]
		public class ColorMatch
		{
			[SerializeField] private TileColor _name;
			[SerializeField] private Color _color;

			public TileColor Name => _name;

			public Color Color => _color;
		}

		[SerializeField] private List<ColorMatch> _configuration;

		public Color this[TileColor color]
		{
			get
			{
				ColorMatch first = null;

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
	}
}