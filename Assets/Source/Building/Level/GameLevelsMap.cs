using System;
using System.Collections.Generic;

namespace TilesWalk.Building.Level
{
	[Serializable]
	public class GameLevelsMap : GenericMap
	{
		/// <summary>
		/// Useful for map building
		/// </summary>
		public List<Tuple<int, string>> Levels;

		public GameLevelsMap()
		{
			Levels = new List<Tuple<int, string>>();
		}

		public GameLevelsMap(GameLevelsMap copyFrom) : base(copyFrom)
		{
			Levels = new List<Tuple<int, string>>(copyFrom.Levels);
		}
	}
}