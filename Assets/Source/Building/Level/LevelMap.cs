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

		public FinishCondition FinishCondition;

		public LevelMap() : base()
		{
		}

		public LevelMap(LevelMap copyFrom) : base(copyFrom)
		{
			this.StarsRequired = copyFrom.StarsRequired;
			Target = copyFrom.Target;
			FinishCondition = copyFrom.FinishCondition;
		}
	}
}