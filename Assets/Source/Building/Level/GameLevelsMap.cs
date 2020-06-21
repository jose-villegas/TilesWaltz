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
		public List<string> TileParameters;

		public GameLevelsMap()
		{
			TileParameters = new List<string>();
		}

		public GameLevelsMap(GameLevelsMap copyFrom) : base(copyFrom)
		{
			TileParameters = new List<string>(copyFrom.TileParameters);
		}
	}
}