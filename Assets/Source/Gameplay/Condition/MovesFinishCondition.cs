using System;

namespace TilesWalk.Gameplay.Condition
{
	[Serializable]
	public class MovesFinishCondition : MapFinishCondition<int>
	{
		protected override int Update(int value)
		{
			_tracker += value;

			if (_tracker >= _limit)
			{
				IsConditionMeet.Value = true;
				return 0;
			}

			return _tracker;
		}

		public MovesFinishCondition(string id, int limit) : base(id, limit)
		{
		}
	}
}