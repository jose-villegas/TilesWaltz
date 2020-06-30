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
		[Serializable]
		public class GameColorMatch : GameColorMatch<GameColor>
		{
		}

		[Serializable]
		private class TileColorMatch : GameColorMatch<TileColor>
		{
		}

		[Serializable]
		private class LevelMapStateMatch : GameColorMatch<LevelMapState>
		{
		}

		[Serializable]
		private class LevelMapStarsMatch : GameColorMatch<int>
		{
		}

		[SerializeField] private List<GameColorMatch> _colorConfiguration;
		[SerializeField] private List<TileColorMatch> _tileConfiguration;
		[SerializeField] private List<LevelMapStateMatch> _mapStateConfiguration;
		[SerializeField] private List<LevelMapStarsMatch> _mapStarsConfiguration;

		public Color GetMapStarsColor(int stars)
		{
			if (_mapStarsConfiguration == null) return Color.black;

			var indexOf = _mapStarsConfiguration.FindIndex(x => x.Match > stars);

			if (indexOf < 0) return Color.black;

			return _mapStarsConfiguration[indexOf].Color;
		}

		public Color this[TileColor color]
		{
			get
			{
				TileColorMatch first = null;

				foreach (var x in _tileConfiguration)
				{
					if (x.Match != color) continue;

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

				foreach (var x in _colorConfiguration)
				{
					if (x.Match != color) continue;

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
				LevelMapStateMatch first = null;

				foreach (var x in _mapStateConfiguration)
				{
					if (x.Match != state) continue;

					first = x;
					break;
				}

				if (first != null) return first.Color;

				return Color.black;
			}
		}
	}
}