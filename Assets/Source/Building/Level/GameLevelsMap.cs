using System;
using System.Collections.Generic;

namespace TilesWalk.Building.Level
{
	[Serializable]
	public class GameLevelsMap : GenericMap
	{
		[Serializable]
		public class GameLevelReference
		{
			public string Id;
			public int Hash;
		}

		/// <summary>
		/// Useful for map building
		/// </summary>
		public List<GameLevelReference> Levels;

		public GameLevelsMap()
		{
			Levels = new List<GameLevelReference>();
		}

		public GameLevelsMap(GameLevelsMap copyFrom) : base(copyFrom)
		{
			Levels = new List<GameLevelReference>(copyFrom.Levels);
		}
	}
}