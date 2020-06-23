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
using UnityEngine.UI;
using Zenject;

namespace TilesWalk.Gameplay.Level.UI
{
	public class LevelFinishDetailsCanvas : CanvasGroupBehaviour
	{
		[Header("Points")] [SerializeField] private TextMeshProUGUI _points;
		[SerializeField] private TextMeshProUGUI _extraPoints;
		[SerializeField] private TextMeshProUGUI _totalPoints;

		[SerializeField] private TextMeshProUGUI _timeExtra;
		[SerializeField] private TextMeshProUGUI _movesExtra;

		[SerializeField] private Button _retry;
		[SerializeField] private Button _continue;

		[Inject] private LevelFinishTracker _levelFinishTracker;
		[Inject] private LevelScorePointsTracker _levelScorePointsTracker;
		[Inject] private MapProviderSolver _solver;
		[Inject] private ScorePointsConfiguration _scorePointsConfiguration;

		private void Start()
		{
			_solver.InstanceProvider(gameObject);
			_levelFinishTracker.OnLevelFinishAsObservable().Subscribe(OnLevelFinish).AddTo(this);
			_continue.onClick.AddListener(_levelScorePointsTracker.SaveScore);
			_retry.onClick.AddListener(_levelScorePointsTracker.SaveScore);
		}

		private void OnLevelFinish(LevelScore score)
		{
			var tileMap = _solver.Provider.Collection.AvailableMaps.Find(x => x.Id == score.Id);

			// points
			_points.text = score.Points.Last.Localize();

			// time
			TimeDetails(score, tileMap);

			// moves
			MovesDetail(score, tileMap);

			Show();
		}

		private void MovesDetail(LevelScore score, LevelMap levelMap)
		{
			if (levelMap.FinishCondition == FinishCondition.MovesLimit)
			{
				var target = score.Moves.Last;
				var limit = _levelFinishTracker.MovesFinishCondition.Limit;

				var extra = ((limit - target) * _scorePointsConfiguration.PointsPerExtraMove);

				_extraPoints.text = extra.Localize();
				_totalPoints.text = (score.Points.Last + extra).Localize();
				_levelScorePointsTracker.AddPoints(extra);
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

				_extraPoints.text = extra.Localize();
				_totalPoints.text = (score.Points.Last + extra).Localize();
				_levelScorePointsTracker.AddPoints(extra);
			}
		}
	}
}