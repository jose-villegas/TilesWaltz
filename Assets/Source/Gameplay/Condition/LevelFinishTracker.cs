using System;
using TilesWalk.Building.Map;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Score;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Condition
{
	public class LevelFinishTracker : ObservableTriggerBase
	{
		[Inject] private ScoreTracker _scoreTracker;
		[Inject] private TileViewMap _tileMap;
		private Subject<TileMap> _onLevelFinish;

		private void Awake()
		{
			_scoreTracker
				.OnScoreUpdatedAsObservable().Subscribe(score =>
				{
					if (score.LastScore >= _tileMap.TileMap.Target)
					{
						_onLevelFinish?.OnNext(_tileMap.TileMap);
					}
				}).AddTo(this);
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			_onLevelFinish?.OnCompleted();
		}

		public IObservable<TileMap> OnLevelFinishAsObservable()
		{
			return _onLevelFinish = _onLevelFinish ?? new Subject<TileMap>();
		}
	}
}