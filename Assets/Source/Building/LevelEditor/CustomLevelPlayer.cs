using System;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Score;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace TilesWalk.Building.LevelEditor
{
	public class CustomLevelPlayer : ObservableTriggerBase
	{
		[Inject] private TileViewLevelMap _tileViewLevelMap;
		[Inject] private LevelScorePointsTracker _levelScorePointsTracker;

		public bool IsPlaying { get; private set; } = false;

		private Subject<LevelMap> _onPlay;
		private Subject<LevelMap> _onStop;

		public void Play()
		{
			IsPlaying = true;
			_levelScorePointsTracker.ResetTrack();
			_onPlay?.OnNext(_tileViewLevelMap.LevelMap);
		}

		public void Stop()
		{
			IsPlaying = false;
			_onStop?.OnNext(_tileViewLevelMap.LevelMap);
		}

		public IObservable<LevelMap> OnPlayAsObservable()
		{
			return _onPlay = _onPlay ?? new Subject<LevelMap>();
		}

		public IObservable<LevelMap> OnStopAsObservable()
		{
			return _onStop = _onStop ?? new Subject<LevelMap>();
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			_onPlay?.OnCompleted();
			_onStop?.OnCompleted();
		}
	}
}