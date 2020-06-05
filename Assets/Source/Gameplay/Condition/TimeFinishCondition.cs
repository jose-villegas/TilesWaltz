using System;
using UnityEngine;

namespace TilesWalk.Gameplay.Condition
{
	[Serializable]
	public class TimeFinishCondition : MapFinishCondition<TimeSpan>
	{
		protected override TimeSpan UpdateHandler(TimeSpan value)
		{
			_handler -= value;

			if (_handler.TotalMilliseconds <= 0)
			{
				IsConditionMeet.Value = true;
				return _handler;
			}

			return _handler;
		}

		public TimeFinishCondition(string id, TimeSpan initial) : base(id, initial)
		{
		}
	}
}