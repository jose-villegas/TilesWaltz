using System;

namespace TilesWalk.Gameplay.Condition
{
	[Serializable]
	public class TargetScoreFinishCondition : MapFinishCondition<int>
	{
		public override int Update(int value)
		{
			_tracker = value;

			if (_tracker >= _limit)
			{
				IsConditionMeet.Value = true;
				return 0;
			}

			return _tracker;
		}

		public TargetScoreFinishCondition(string id, int limit) : base(id, limit)
		{
		}
	}
}