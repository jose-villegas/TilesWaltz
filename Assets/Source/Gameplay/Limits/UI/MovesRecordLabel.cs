using System.Collections.Generic;
using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Condition;
using TilesWalk.Gameplay.Score;
using TilesWalk.General.Patterns;
using TilesWalk.Navigation.UI;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Limits.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class MovesRecordLabel : ObligatoryComponentBehaviour<TextMeshProUGUI>
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
			if (levelMap.FinishCondition != FinishCondition.MovesLimit)
			{
				transform.parent.gameObject.SetActive(false);
				return;
			}

			var condition = _levelFinishTracker.MovesFinishCondition;
			Component.text = $"{_levelScorePointsTracker.LevelScore.Moves.Lowest.Localize()}";
			condition.Tracker.SubscribeToText(Component,
					value => $"{_levelScorePointsTracker.LevelScore.Moves.Lowest.Localize()}")
				.AddTo(this);
		}
	}
}