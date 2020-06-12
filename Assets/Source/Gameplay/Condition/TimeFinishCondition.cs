using System;
using UnityEngine;

namespace TilesWalk.Gameplay.Condition
{
	[Serializable]
	public class TimeFinishCondition : MapFinishCondition<float>
	{
		public override float Update(float value)
		{
			_tracker.Value += value;

			if (_tracker.Value >= _limit)
			{
				IsConditionMeet.Value = true;
				return _tracker.Value;
			}

			return _tracker.Value;
		}

		public TimeFinishCondition(string id, float limit) : base(id, limit)
		{
		}
	}
}