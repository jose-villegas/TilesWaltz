using System;
using System.Collections.Generic;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Input;
using TilesWalk.Gameplay.Score;
using TilesWalk.Map.General;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Condition
{
	public class LevelFinishTracker : ObservableTriggerBase
	{
		[Inject] private LevelScorePointsTracker _levelScorePointsTracker;
		[Inject] private TileViewLevelMap _tileLevelMap;
		[Inject] private MapProviderSolver _solver;
		[Inject] private GameEventsHandler _gameEvents;

		private TargetScorePointsCondition _targetPointsCondition;
		private MovesFinishCondition _movesFinishCondition;
		private TimeFinishCondition _timeFinishCondition;

		private Subject<LevelScore> _onLevelFinish;
		private Subject<LevelScore> _onTrackersSetupFinish;
		private Subject<LevelScore> _onScorePointsTargetReached;
		private bool _gamePaused;

		public MovesFinishCondition MovesFinishCondition => _movesFinishCondition;
		public TimeFinishCondition TimeFinishCondition => _timeFinishCondition;

		public bool IsFinished { get; private set; } = false;

		private void Awake()
		{
			_solver.InstanceProvider(gameObject);
			_tileLevelMap.OnLevelMapLoadedAsObservable().Subscribe(OnLevelMapLoaded).AddTo(this);
			_gameEvents.OnGamePausedAsObservable().Subscribe(OnGamePaused).AddTo(this);
			_gameEvents.OnGameResumedAsObservable().Subscribe(OnGameResumed).AddTo(this);
		}

		private void OnGameResumed(Unit obj)
		{
			_gamePaused = false;
		}

		private void OnGamePaused(Unit obj)
		{
			_gamePaused = true;
		}

		private void OnLevelMapLoaded(LevelMap levelMap)
		{
			_targetPointsCondition = new TargetScorePointsCondition(levelMap.Id, levelMap.Target);

			// track score points
			_levelScorePointsTracker.OnScorePointsUpdatedAsObservable().Subscribe(score =>
				{
					_targetPointsCondition.Update(score.Points.Last);
				})
				.AddTo(this);

			// track target points completion
			_targetPointsCondition.IsConditionMeet.Subscribe(meet =>
			{
				if (!meet) return;

				_onScorePointsTargetReached?.OnNext(_levelScorePointsTracker.LevelScore);
			}).AddTo(this);

			if (levelMap.FinishCondition == FinishCondition.MovesLimit)
			{
				// find finish conditions
				_movesFinishCondition =
					_solver.Provider.Collection.MovesFinishConditions.Find(x => x.Id == levelMap.Id);
				_movesFinishCondition.Reset(0);
				// track moves
				MovesTracking();
			}
			else if (levelMap.FinishCondition == FinishCondition.TimeLimit)
			{
				_timeFinishCondition = _solver.Provider.Collection.TimeFinishConditions.Find(x => x.Id == levelMap.Id);
				_timeFinishCondition.Reset(0);
				// track time
				TimeTracking();
			}

			_onTrackersSetupFinish?.OnNext(_levelScorePointsTracker.LevelScore);
		}

		private void MovesTracking()
		{
			_tileLevelMap.OnTileRemovedAsObservable().Subscribe(l =>
				{
					if (_gamePaused || _timeFinishCondition.IsConditionMeet.Value) return;

					_movesFinishCondition?.Update(1);
				})
				.AddTo(this);

			_movesFinishCondition.IsConditionMeet.Subscribe(meet =>
			{
				if (!meet) return;

				if (_movesFinishCondition != null)
				{
					_levelScorePointsTracker.LevelScore.Moves.Update(_movesFinishCondition.Tracker.Value);
				}

				IsFinished = true;
				_onLevelFinish?.OnNext(_levelScorePointsTracker.LevelScore);
			}).AddTo(this);
		}

		private void TimeTracking()
		{
			Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(l =>
			{
				if (_gamePaused || _timeFinishCondition.IsConditionMeet.Value) return;

				_timeFinishCondition?.Update(1f);
			}).AddTo(this);

			_timeFinishCondition.IsConditionMeet.Subscribe(meet =>
			{
				if (!meet) return;

				if (_timeFinishCondition != null)
				{
					_levelScorePointsTracker.LevelScore.Time.Update(_timeFinishCondition.Tracker.Value);
				}

				IsFinished = true;
				_onLevelFinish?.OnNext(_levelScorePointsTracker.LevelScore);
			}).AddTo(this);
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			_onLevelFinish?.OnCompleted();
			_onScorePointsTargetReached?.OnCompleted();
			_onTrackersSetupFinish?.OnCompleted();
		}

		public IObservable<LevelScore> OnTrackersSetupFinishAsObservable()
		{
			return _onTrackersSetupFinish = _onTrackersSetupFinish ?? new Subject<LevelScore>();
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