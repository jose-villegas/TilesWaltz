using System;
using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Condition;
using TilesWalk.Gameplay.Score;
using TilesWalk.General.Patterns;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Limits.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class TimeRecordLabel : ObligatoryComponentBehaviour<TextMeshProUGUI>
	{
		[Inject] private TileViewLevelMap _tileLevelMap;
		[Inject] private LevelFinishTracker _levelFinishTracker;
		[Inject] private LevelScorePointsTracker _levelScorePointsTracker;

		private void Start()
		{
			_tileLevelMap
				.OnLevelMapLoadedAsObservable()
				.Subscribe(
					OnLevelMapLoaded
				)
				.AddTo(this);
		}

		private void OnLevelMapLoaded(LevelMap levelMap)
		{
			if (levelMap.FinishCondition != FinishCondition.TimeLimit)
			{
				transform.parent.gameObject.SetActive(false);
				return;
			}

			transform.UpdateAsObservable().SubscribeToText(Component, _ =>
			{
				var current = new DateTime(TimeSpan.FromSeconds(_levelScorePointsTracker.LevelScore.Time.Lowest).Ticks);
				var currentTime = current.ToString("mm:ss");
				return $"{currentTime}";
			}).AddTo(this);
		}
	}
}