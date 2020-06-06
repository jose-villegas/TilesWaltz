using System;
using System.Collections.Generic;
using TilesWalk.Building.Map;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Score;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Timeline;
using Zenject;

namespace TilesWalk.Gameplay.Condition
{
	public class LevelFinishTracker : ObservableTriggerBase
	{
		[Inject] private LevelScoreTracker _levelScoreTracker;
		[Inject] private TileViewMap _tileMap;
		[Inject] private List<MovesFinishCondition> _movesFinishConditions;
		[Inject] private List<TimeFinishCondition> _timeFinishConditions;

		private TargetScoreFinishCondition _targetFinishCondition;
		private MovesFinishCondition _movesFinishCondition;
		private TimeFinishCondition _timeFinishCondition;

		private Subject<TileMap> _onLevelFinish;

		public MovesFinishCondition MovesFinishCondition => _movesFinishCondition;
		public TimeFinishCondition TimeFinishCondition => _timeFinishCondition;

		private void Awake()
		{
			_tileMap.OnTileMapLoadedAsObservable().Subscribe(OnTileMapLoaded).AddTo(this);
		}

		private void OnTileMapLoaded(TileMap tileMap)
		{
			_targetFinishCondition = new TargetScoreFinishCondition(tileMap.Id, tileMap.Target);

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
			_levelScoreTracker
				.OnScorePointsUpdatedAsObservable().Subscribe(score =>
				{
					_targetFinishCondition.Update(score.Points.Last);
				})
				.AddTo(this);

			// track time
			transform.UpdateAsObservable().Subscribe(_ =>
				{
					_timeFinishCondition?.Update(Time.deltaTime);

					if (_tileMap.TileMap.FinishCondition == FinishCondition.TimeLimit)
					{
						if (_timeFinishCondition.IsConditionMeet.Value)
						{
							_onLevelFinish?.OnNext(_tileMap.TileMap);
						}
					}
				})
				.AddTo(this);

			// track moves
			_tileMap.OnTileRemovedAsObservable().Subscribe(_ =>
			{
				_movesFinishCondition?.Update(1);

				if (_tileMap.TileMap.FinishCondition == FinishCondition.MovesLimit)
				{
					if (_movesFinishCondition.IsConditionMeet.Value)
					{
						_onLevelFinish?.OnNext(_tileMap.TileMap);
					}
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