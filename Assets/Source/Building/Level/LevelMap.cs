using System;
using System.Collections.Generic;
using TilesWalk.BaseInterfaces;
using TilesWalk.Gameplay.Condition;

namespace TilesWalk.Building.Level
{
	[Serializable]
	public class LevelMap : GenericMap
	{
		public int StarsRequired;
		public int Target;
		public int MapSize;
		public FinishCondition FinishCondition;

		public LevelMap()
		{
			Id = "-1";
			Instructions = new List<InsertionInstruction>();
			Tiles = new List<int>();
		}
	}
}