using System;
using System.Collections.Generic;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Score;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Condition
{
	public class LevelFinishTracker : ObservableTriggerBase
	{
		[Inject] private LevelScorePointsTracker _levelScorePointsTracker;
		[Inject] private TileViewMap _tileMap;
		[Inject] private List<MovesFinishCondition> _movesFinishConditions;
		[Inject] private List<TimeFinishCondition> _timeFinishConditions;

		private TargetScorePointsCondition _targetPointsCondition;
		private MovesFinishCondition _movesFinishCondition;
		private TimeFinishCondition _timeFinishCondition;

		private Subject<LevelScore> _onLevelFinish;
		private Subject<LevelScore> _onScorePointsTargetReached;

		public MovesFinishCondition MovesFinishCondition => _movesFinishCondition;
		public TimeFinishCondition TimeFinishCondition => _timeFinishCondition;

		private void Awake()
		{
			_tileMap.OnTileMapLoadedAsObservable().Subscribe(OnTileMapLoaded).AddTo(this);
		}

		private void OnTileMapLoaded(TileMap tileMap)
		{
			_targetPointsCondition = new TargetScorePointsCondition(tileMap.Id, tileMap.Target);

			// find finish conditions
			_movesFinishCondition = _movesFinishConditions.Find(x => x.Id == tileMap.Id);
			_timeFinishCondition = _timeFinishConditions.Find(x => x.Id == tileMap.Id);

			if (_movesFinishCondition == null)
			{
				_movesFinishCondition = new MovesFinishCondition(tileMap.Id, int.MaxValue);
			}

			if (_timeFinishCondition == null)
			{
				_timeFinishCondition = new TimeFinishCondition(tileMap.Id, float.MaxValue);
			}

			_movesFinishCondition.Reset(0);
			_timeFinishCondition.Reset(0);

			// track score points
			_levelScorePointsTracker
				.OnScorePointsUpdatedAsObservable().Subscribe(score =>
				{
					_targetPointsCondition.Update(score.Points.Last);
				})
				.AddTo(this);

			// track target points completion
			_targetPointsCondition.IsConditionMeet.Subscribe(meet =>
			{
				if (!meet) return;

				_levelScorePointsTracker.LevelScore.Moves.Update(_movesFinishCondition.Tracker);
				_levelScorePointsTracker.LevelScore.Time.Update(_timeFinishCondition.Tracker);
				_onScorePointsTargetReached?.OnNext(_levelScorePointsTracker.LevelScore);
			}).AddTo(this);

			// track time
			TimeTracking(tileMap);

			// track moves
			MovesTracking();
		}

		private void MovesTracking()
		{
			_tileMap.OnTileRemovedAsObservable().Subscribe(_ => { _movesFinishCondition?.Update(1); })
				.AddTo(this);

			if (_tileMap.TileMap.FinishCondition == FinishCondition.MovesLimit)
			{
				_movesFinishCondition.IsConditionMeet.Subscribe(meet =>
				{
					if (!meet) return;

					if (!_targetPointsCondition.IsConditionMeet.Value)
					{
						_levelScorePointsTracker.LevelScore.Moves.Update(_movesFinishCondition.Limit);
						_levelScorePointsTracker.LevelScore.Time.Update(_timeFinishCondition.Tracker);
					}

					_onLevelFinish?.OnNext(_levelScorePointsTracker.LevelScore);
				}).AddTo(this);
			}
		}

		private void TimeTracking(TileMap tileMap)
		{
			transform.UpdateAsObservable().Subscribe(_ =>
				{
					if (!_timeFinishCondition.IsConditionMeet.Value)
					{
						_timeFinishCondition?.Update(Time.deltaTime);
					}
				})
				.AddTo(this);

			if (tileMap.FinishCondition == FinishCondition.TimeLimit)
			{
				_timeFinishCondition.IsConditionMeet.Subscribe(meet =>
				{
					if (!meet) return;

					if (!_targetPointsCondition.IsConditionMeet.Value)
					{
						_levelScorePointsTracker.LevelScore.Moves.Update(_movesFinishCondition.Tracker);
						_levelScorePointsTracker.LevelScore.Time.Update(_timeFinishCondition.Limit);
					}

					_onLevelFinish?.OnNext(_levelScorePointsTracker.LevelScore);
				}).AddTo(this);
			}
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			_onLevelFinish?.OnCompleted();
			_onScorePointsTargetReached?.OnCompleted();
		}

		public IObservable<LevelScore> OnLevelFinishAsObservable()
		{
			return _onLevelFinish = _onLevelFinish ?? new Subject<LevelScore>();
		}

		public IObservable<LevelScore> OnScorePointsTargetReachedAsObservable()
		{
			return _onScorePointsTargetReached = _onScorePointsTargetReached ?? new Subject<LevelScore>();
		}
	}
}