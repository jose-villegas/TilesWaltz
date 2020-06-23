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
	public class LevelFinishTimeLabel : ObligatoryComponentBehaviour<TextMeshProUGUI>
	{
		[Inject] private LevelFinishTracker _finishTracker;

		private void Start()
		{
			_finishTracker.OnLevelFinishAsObservable().Subscribe(OnLevelFinish).AddTo(this);
		}

		private void OnLevelFinish(LevelScore score)
		{
			var target = TimeSpan.FromSeconds(score.Time.Last);
			Component.text = new DateTime(target.Ticks).ToString("mm:ss");
		}
	}
}