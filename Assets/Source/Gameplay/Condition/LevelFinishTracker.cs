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
		[Inject] private LevelScoreTracker _levelScoreTracker;
		[Inject] private TileViewMap _tileMap;

		private Subject<TileMap> _onLevelFinish;
		private TargetScoreFinishCondition _targetFinish;

		private void Awake()
		{
			_tileMap.OnTileMapLoadedAsObservable().Subscribe(OnTileMapLoaded).AddTo(this);
		}

		private void OnTileMapLoaded(TileMap tileMap)
		{
			_targetFinish = new TargetScoreFinishCondition(tileMap.Id, tileMap.Target);
			_levelScoreTracker
				.OnScoreUpdatedAsObservable().Subscribe(score => { _targetFinish.Update(score.Points.Last); })
				.AddTo(this);
			_targetFinish.IsConditionMeet.Subscribe(meet =>
			{
				if (meet) _onLevelFinish?.OnNext(_tileMap.TileMap);
			});
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