using System;
using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Condition;
using TilesWalk.General;
using TilesWalk.General.Patterns;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace TilesWalk.Building.LevelEditor.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class CustomLevelTimeTrackerLabel : ObligatoryComponentBehaviour<TextMeshProUGUI>
	{
		[Inject] private CustomLevelPlayer _customLevelPlayer;
		private TimeFinishCondition _condition;
		private IDisposable _update;

		private void Start()
		{
			Component.text = "00:00";
			_condition = new TimeFinishCondition(Constants.CustomLevelName, float.MaxValue);
			_customLevelPlayer.OnPlayAsObservable().Subscribe(OnCustomLevelPlay).AddTo(this);
			_customLevelPlayer.OnStopAsObservable().Subscribe(OnCustomLevelStop).AddTo(this);
		}

		private void OnCustomLevelStop(LevelMap obj)
		{
			_update.Dispose();
		}

		private void OnCustomLevelPlay(LevelMap obj)
		{
			Component.text = "00:00";
			_condition.Reset(0);

			_update = transform.UpdateAsObservable().Subscribe(_ =>
			{
				_condition.Update(Time.deltaTime);
				Component.text = new DateTime(TimeSpan.FromSeconds(_condition.Tracker.Value).Ticks).ToString(("mm:ss"));
			});
		}
	}
}