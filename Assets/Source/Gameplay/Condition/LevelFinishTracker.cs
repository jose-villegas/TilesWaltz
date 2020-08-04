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

		private MovesFinishCondition _movesFinishCondition;
		private TimeFinishCondition _timeFinishCondition;

		private Subject<LevelScore> _onLevelFinish;
		private Subject<LevelScore> _onTrackersSetupFinish;
		private bool _gamePaused;
		private bool _recordSet;

		public MovesFinishCondition MovesFinishCondition => _movesFinishCondition;
		public TimeFinishCondition TimeFinishCondition => _timeFinishCondition;

		public bool IsFinished { get; private set; } = false;

		private void Awake()
		{
			_solver.InstanceProvider(gameObject);
			_tileLevelMap.OnLevelMapDataLoadedAsObservable().Subscribe(OnLevelMapLoaded).AddTo(this);
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

		/// <summary>
		/// Initializes tracking logic
		/// </summary>
		/// <param name="levelMap"></param>
		private void OnLevelMapLoaded(LevelMap levelMap)
		{
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

		/// <summary>
		/// Handles level finish on moves limit condition
		/// </summary>
		private void MovesTracking()
		{
			_tileLevelMap.Trigger.OnTileRemovedAsObservable().Subscribe(l =>
				{
					if (_gamePaused || _movesFinishCondition.IsConditionMeet.Value) return;

					_movesFinishCondition?.Update(1);
				})
				.AddTo(this);

			_levelScorePointsTracker.OnScorePointsUpdatedAsObservable().Subscribe(score =>
			{
				if (score.Points.Last < _tileLevelMap.Map.Target) return;

				if (_movesFinishCondition != null && !_recordSet)
				{
					_recordSet = true;
					_levelScorePointsTracker.LevelScore.Moves.Update(_movesFinishCondition.Tracker.Value);
				}
			});

			_movesFinishCondition.IsConditionMeet.Subscribe(meet =>
			{
				if (!meet) return;

				if (_movesFinishCondition != null && !_recordSet)
				{
					_recordSet = true;
					_levelScorePointsTracker.LevelScore.Moves.Update(_movesFinishCondition.Limit);
				}

				TriggerLevelFinish();
			}).AddTo(this);
		}

		/// <summary>
		/// Handles level finishing logic, since some combos may still on level finish
		/// its important to handle this properly
		/// </summary>
		private void TriggerLevelFinish()
		{
			IsFinished = true;

			if (_tileLevelMap.IsAnyComboLeft())
			{
				_levelScorePointsTracker.OnScorePointsUpdatedAsObservable().Subscribe(score =>
				{
					// wait till the map is unlocked
					if (_tileLevelMap.State == TileLevelMapState.FreeMove)
					{
						_onLevelFinish?.OnNext(_levelScorePointsTracker.LevelScore);
					}
				}).AddTo(this);
			}
			else
			{
				_onLevelFinish?.OnNext(_levelScorePointsTracker.LevelScore);
			}
		}

		/// <summary>
		/// Handles level finish on time limit logic
		/// </summary>
		private void TimeTracking()
		{
			Observable.Interval(TimeSpan.FromMilliseconds(10)).Subscribe(l =>
			{
				if (_gamePaused || _timeFinishCondition.IsConditionMeet.Value) return;

				_timeFinishCondition?.Update(10f / 1000f);
			}).AddTo(this);

			// this could be done using TakeWhile, when UniRX supports inclusive last value
			_levelScorePointsTracker.OnScorePointsUpdatedAsObservable().Subscribe(score =>
			{
				if (score.Points.Last < _tileLevelMap.Map.Target) return;

				if (_timeFinishCondition != null && !_recordSet)
				{
					_recordSet = true;
					_levelScorePointsTracker.LevelScore.Time.Update(_timeFinishCondition.Tracker.Value);
				}
			}).AddTo(this);


			_timeFinishCondition.IsConditionMeet.Subscribe(meet =>
			{
				if (!meet) return;

				if (_timeFinishCondition != null && !_recordSet)
				{
					_recordSet = true;
					_levelScorePointsTracker.LevelScore.Time.Update(_timeFinishCondition.Limit);
				}

				TriggerLevelFinish();
			}).AddTo(this);
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			_onLevelFinish?.OnCompleted();
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
	}
}