﻿using System;

namespace TilesWalk.Gameplay.Condition
{
	[Serializable]
	public class TargetScorePointsCondition : MapFinishCondition<int>
	{
		public override int Update(int value)
		{
			_tracker.Value = value;

			if (_tracker.Value >= _limit)
			{
				IsConditionMeet.Value = true;
				return 0;
			}

			return _tracker.Value;
		}

		public TargetScorePointsCondition(string id, int limit) : base(id, limit)
		{
		}
	}
}