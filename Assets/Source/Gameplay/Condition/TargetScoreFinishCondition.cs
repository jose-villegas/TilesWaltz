using System;

namespace TilesWalk.Gameplay.Condition
{
	[Serializable]
	public class TargetScoreFinishCondition : MapFinishCondition<int>
	{
		protected override int UpdateHandler(int value)
		{
			_handler -= value;

			if (_handler <= 0)
			{
				IsConditionMeet.Value = true;
				return 0;
			}

			return _handler;
		}

		public TargetScoreFinishCondition(string id, int initial) : base(id, initial)
		{
		}
	}
}