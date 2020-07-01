using System;
using System.Collections.Generic;
using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Condition;
using TilesWalk.Gameplay.Input;
using TilesWalk.Gameplay.Score;
using TilesWalk.General;
using TilesWalk.General.Patterns;
using TilesWalk.Navigation.UI;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Limits.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class TimeTrackerLabel : ObligatoryComponentBehaviour<TextMeshProUGUI>
	{
		[Inject] private TileViewLevelMap _tileLevelMap;
		[Inject] private LevelFinishTracker _levelFinishTracker;
		[Inject] private GameEventsHandler _gameEvents;
		private bool _gamePaused;

		private void Awake()
		{
			_gameEvents.OnGamePausedAsObservable().Subscribe(OnGamePaused).AddTo(this);
			_gameEvents.OnGameResumedAsObservable().Subscribe(OnGameResumed).AddTo(this);
			_levelFinishTracker.OnTrackersSetupFinishAsObservable().Subscribe(OnLevelMapLoaded).AddTo(this);
		}

		private void OnGameResumed(Unit obj)
		{
			_gamePaused = false;
		}

		private void OnGamePaused(Unit obj)
		{
			_gamePaused = true;
		}

		private void OnLevelMapLoaded(LevelScore score)
		{
			var condition = _levelFinishTracker.TimeFinishCondition;

			if (condition == null) return;

			var end = TimeSpan.FromSeconds(condition.Limit);
			var seconds = 0f;

			Component.text = $"00:00/{new DateTime(end.Ticks).ToString("mm:ss")}";

			Observable.Interval(TimeSpan.FromSeconds(1)).SubscribeToText(Component, l =>
			{
				var current = new DateTime(TimeSpan.FromSeconds(seconds).Ticks).ToString("mm:ss");
				var limit = new DateTime(end.Ticks).ToString("mm:ss");

				if (_levelFinishTracker.IsFinished) return $"{limit}/{limit}";

				if (_gamePaused) return $"{current}/{limit}";

				seconds++;

				return seconds < condition.Limit ? $"{current}/{limit}" : $"{limit}/{limit}";
			}).AddTo(this);
		}
	}
}