using System;
using UnityEngine;

namespace TilesWalk.Gameplay.Condition
{
	[Serializable]
	public class TimeFinishCondition : MapFinishCondition<float>
	{
		public override float Update(float value)
		{
			_tracker += value;

			if (_tracker >= _limit)
			{
				IsConditionMeet.Value = true;
				return _tracker;
			}

			return _tracker;
		}

		public TimeFinishCondition(string id, float limit) : base(id, limit)
		{
		}
	}
}