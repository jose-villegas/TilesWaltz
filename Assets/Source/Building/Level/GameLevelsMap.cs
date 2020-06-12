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
			Id = "-1";
			Instructions = new List<InsertionInstruction>();
			TileParameters = new List<string>();
			Tiles = new List<int>();
		}
	}
}