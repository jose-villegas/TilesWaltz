using System;
using TilesWalk.Gameplay.Condition;
using TilesWalk.Gameplay.Score;
using TilesWalk.General.Patterns;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Level.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class LevelFinishExtraTimeLabel : ObligatoryComponentBehaviour<TextMeshProUGUI>
	{
		[Inject] private LevelFinishTracker _finishTracker;

		private void Start()
		{
			_finishTracker.OnLevelFinishAsObservable().Subscribe(OnLevelFinish).AddTo(this);
		}

		private void OnLevelFinish(LevelScore score)
		{
			var target = TimeSpan.FromSeconds(score.Time.Last);
			var limit = TimeSpan.FromSeconds(_finishTracker.TimeFinishCondition.Limit);
			var ticks = (limit - target).Ticks;

			if (ticks >= DateTime.MaxValue.Ticks)
			{
				ticks = DateTime.MaxValue.Ticks;
			}
			else if (ticks <= DateTime.MinValue.Ticks)
			{
				ticks = DateTime.MinValue.Ticks;
			}

			Component.text = new DateTime(ticks).ToString("mm:ss");
		}
	}
}