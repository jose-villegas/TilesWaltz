using System;
using System.Collections.Generic;
using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Condition;
using TilesWalk.Gameplay.Score;
using TilesWalk.General.UI;
using TilesWalk.Map.Bridge;
using TilesWalk.Map.General;
using TilesWalk.Navigation.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace TilesWalk.Gameplay.Level.UI
{
	public class LevelFinishDetailsCanvas : CanvasGroupBehaviour
	{
		[Inject] private LevelFinishTracker _levelFinishTracker;
		[Inject] private LevelScorePointsTracker _levelScorePointsTracker;
		[Inject] private TileViewLevelMap _levelMap;
		[Inject] private ScorePointsConfiguration _scorePointsConfiguration;

		[Header("Points")] [SerializeField] private SlidingNumber _points;
		[SerializeField] private SlidingNumber _extraPoints;
		[SerializeField] private SlidingNumber _totalPoints;

		[SerializeField] private TextMeshProUGUI _timeExtra;
		[SerializeField] private TextMeshProUGUI _movesExtra;

		[SerializeField] private Button _retry;
		[SerializeField] private Button _continue;

		public void OnLevelFinish(LevelScore score)
		{
			// points
			_points.Current = 0;

			// time
			TimeDetails(score, _levelMap.Map);

			// moves
			MovesDetail(score, _levelMap.Map);

			_retry.interactable = false;
			_continue.interactable = false;

			Show();
		}

		private void MovesDetail(LevelScore score, LevelMap levelMap)
		{
			if (levelMap.FinishCondition == FinishCondition.MovesLimit)
			{
				var target = score.Moves.Last;
				var limit = _levelFinishTracker.MovesFinishCondition.Limit;

				var extra = ((limit - target) * _scorePointsConfiguration.PointsPerExtraMove);

				_extraPoints.Current = 0;
				_totalPoints.Current = score.Points.Last - extra;
				_points.Target(score.Points.Last - extra);

				_points.OnTargetReachedAsObservable().Subscribe(p =>
				{
					_extraPoints.OnTargetReachedAsObservable().Subscribe(e =>
					{
						_totalPoints.Target(score.Points.Last);

						_totalPoints.OnTargetReachedAsObservable().Subscribe(_ =>
						{
							_retry.interactable = true;
							_continue.interactable = true;
						}).AddTo(this);
					}).AddTo(this);

					_extraPoints.Target(extra);
				}).AddTo(this);
			}
		}

		private void TimeDetails(LevelScore score, LevelMap levelMap)
		{
			if (levelMap.FinishCondition == FinishCondition.TimeLimit)
			{
				var target = TimeSpan.FromSeconds(score.Time.Last);
				var limit = TimeSpan.FromSeconds(_levelFinishTracker.TimeFinishCondition.Limit);

				var extra = Mathf.RoundToInt((float) (limit - target).TotalSeconds) *
				            _scorePointsConfiguration.PointsPerExtraSecond;

				_extraPoints.Current = 0;
				_totalPoints.Current = score.Points.Last - extra;
				_points.Target(score.Points.Last - extra);

				_points.OnTargetReachedAsObservable().Subscribe(p =>
				{
					_extraPoints.OnTargetReachedAsObservable().Subscribe(e =>
					{
						_totalPoints.Target(score.Points.Last);

						_totalPoints.OnTargetReachedAsObservable().Subscribe(_ =>
						{
							_retry.interactable = true;
							_continue.interactable = true;
						}).AddTo(this);
					}).AddTo(this);

					_extraPoints.Target(extra);
				}).AddTo(this);
			}
		}
	}
}