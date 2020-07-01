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
	public class LevelFinishTimeLimitLabel : ObligatoryComponentBehaviour<TextMeshProUGUI>
	{
		[Inject] private LevelFinishTracker _finishTracker;

		private void Start()
		{
			_finishTracker.OnLevelFinishAsObservable().Subscribe(OnLevelFinish).AddTo(this);
		}

		private void OnLevelFinish(LevelScore score)
		{
			var limit = TimeSpan.FromSeconds(_finishTracker.TimeFinishCondition.Limit);
			Component.text = string.Format("{0:mm\\:ss}", limit);
		}
	}
}