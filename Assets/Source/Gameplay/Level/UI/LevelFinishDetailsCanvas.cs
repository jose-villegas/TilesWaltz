using System;
using System.Collections.Generic;
using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Condition;
using TilesWalk.Gameplay.Score;
using TilesWalk.General.UI;
using TilesWalk.Map.Bridge;
using TilesWalk.Navigation.UI;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Level.UI
{
	public class LevelFinishDetailsCanvas : CanvasGroupBehaviour
	{
		[SerializeField] private TextMeshProUGUI _points;
		[Header("Time")] 
		[SerializeField] private GameObject _timeContainer;
		[SerializeField] private TextMeshProUGUI _timeLimit;
		[SerializeField] private TextMeshProUGUI _timeTarget;
		[SerializeField] private TextMeshProUGUI _timeExtra;
		[Header("Moves")]
		[SerializeField] private GameObject _movesContainer;
		[SerializeField] private TextMeshProUGUI _movesLimit;
		[SerializeField] private TextMeshProUGUI _movesTarget;
		[SerializeField] private TextMeshProUGUI _movesExtra;

		[Header("Result")]
		[SerializeField] private TextMeshProUGUI _extraPoints;
		[SerializeField] private TextMeshProUGUI _totalPoints;

		[Inject] private LevelFinishTracker _levelFinishTracker;
		[Inject] private LevelScorePointsTracker _levelScorePointsTracker;
		[Inject] private List<TileMap> _availableMaps;
		[Inject] private ScorePointsConfiguration _scorePointsConfiguration;

		private void Start()
		{
			_levelFinishTracker.OnLevelFinishAsObservable().Subscribe(OnLevelFinish).AddTo(this);
		}

		private void OnLevelFinish(LevelScore score)
		{
			var tileMap = _availableMaps.Find(x => x.Id == score.Id);

			// points
			_points.text = score.Points.Last.Localize();

			// time
			TimeDetails(score, tileMap);

			// moves
			MovesDetail(score, tileMap);

			Show();
		}

		private void MovesDetail(LevelScore score, TileMap tileMap)
		{
			var target = score.Moves.Last;
			var limit = _levelFinishTracker.MovesFinishCondition.Limit;

			if (tileMap.FinishCondition == FinishCondition.MovesLimit)
			{
				_timeLimit.text = limit.Localize();
				_timeTarget.text = target.Localize();
				_timeExtra.text = (limit - target).Localize();

				var extra = ((limit - target) * _scorePointsConfiguration.PointsPerExtraMove);
				_extraPoints.text = extra.Localize();
				_totalPoints.text = (score.Points.Last + extra).Localize();
				_levelScorePointsTracker.AddPoints(extra);
			}
			else
			{
				_movesContainer.transform.gameObject.SetActive(false);
			}
		}

		private void TimeDetails(LevelScore score, TileMap tileMap)
		{
			var start = DateTime.Now;
			var target = TimeSpan.FromSeconds(score.Time.Last);
			var limit = TimeSpan.FromSeconds(_levelFinishTracker.TimeFinishCondition.Limit);

			if (tileMap.FinishCondition == FinishCondition.TimeLimit)
			{
				_timeLimit.text = new DateTime(limit.Ticks).ToString("mm:ss");
				_timeTarget.text = new DateTime(target.Ticks).ToString("mm:ss");
				_timeExtra.text = new DateTime((limit - target).Ticks).ToString("mm:ss");

				var extra = Mathf.RoundToInt((float)(limit - target).TotalSeconds) * 
				            _scorePointsConfiguration.PointsPerExtraSecond;
				_extraPoints.text = extra.Localize();
				_totalPoints.text = (score.Points.Last + extra).Localize();
				_levelScorePointsTracker.AddPoints(extra);
			}
			else
			{
				_timeContainer.transform.gameObject.SetActive(false);
			}
		}
	}
}