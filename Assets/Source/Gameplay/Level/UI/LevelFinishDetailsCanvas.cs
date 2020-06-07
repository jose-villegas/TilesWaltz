using System;
using System.Collections.Generic;
using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Condition;
using TilesWalk.Gameplay.Score;
using TilesWalk.General.UI;
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
		[SerializeField] private TextMeshProUGUI _time;
		[SerializeField] private TextMeshProUGUI _moves;
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
				_time.text = string.Format("{0}-\n" + "<u>{1}</u>\n" + "{2}", limit, target, limit - target);

				var extra = ((limit - target) * _scorePointsConfiguration.PointsPerExtraMove);
				_extraPoints.text = extra.Localize();
				_totalPoints.text = (score.Points.Last + extra).Localize();
				_levelScorePointsTracker.AddPoints(extra);
			}
			else
			{
				_moves.transform.parent.gameObject.SetActive(false);
			}
		}

		private void TimeDetails(LevelScore score, TileMap tileMap)
		{
			var start = DateTime.Now;
			var target = TimeSpan.FromSeconds(score.Time.Last);
			var limit = TimeSpan.FromSeconds(_levelFinishTracker.TimeFinishCondition.Limit);

			if (tileMap.FinishCondition == FinishCondition.TimeLimit)
			{
				_time.text = string.Format("{0}-\n" + "<u>{1}</u>\n" + "{2}",
					new DateTime(limit.Ticks).ToString("mm:ss"),
					new DateTime(target.Ticks).ToString("mm:ss"),
					new DateTime((limit - target).Ticks).ToString("mm:ss"));

				var extra = Mathf.RoundToInt((float)(limit - target).TotalSeconds) * 
				            _scorePointsConfiguration.PointsPerExtraSecond;
				_extraPoints.text = extra.Localize();
				_totalPoints.text = (score.Points.Last + extra).Localize();
				_levelScorePointsTracker.AddPoints(extra);
			}
			else
			{
				_time.transform.parent.gameObject.SetActive(false);
			}
		}
	}
}