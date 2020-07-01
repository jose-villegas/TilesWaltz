using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace TilesWalk.Gameplay.Input
{
	public class GameEventsHandler : ObservableTriggerBase
	{
		private Subject<Unit> _onGamePaused;
		private Subject<Unit> _onGameResumed;

		public void Resume()
		{
			Debug.Log("Game Resumed");
			_onGameResumed?.OnNext(new Unit());
		}

		public void Pause()
		{
			Debug.Log("Game Paused");
			_onGamePaused?.OnNext(new Unit());
		}

		public IObservable<Unit> OnGamePausedAsObservable()
		{
			return _onGamePaused = _onGamePaused == null ? new Subject<Unit>() : _onGamePaused;
		}

		public IObservable<Unit> OnGameResumedAsObservable()
		{
			return _onGameResumed = _onGameResumed == null ? new Subject<Unit>() : _onGameResumed;
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
		}
	}
}