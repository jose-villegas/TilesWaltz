using System.Collections.Generic;
using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Condition;
using TilesWalk.General.Patterns;
using TilesWalk.Navigation.UI;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Limits.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class MovesCounterLabel : ObligatoryComponentBehaviour<TextMeshProUGUI>
	{
		[Inject] private TileViewLevelMap _tileLevelMap;
		[Inject] private LevelFinishTracker _levelFinishTracker;

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
			Component.text = $"{condition.Tracker.Value}/{condition.Limit.Localize()}";
			condition.Tracker.SubscribeToText(Component, value => $"{value.Localize()}/{condition.Limit.Localize()}")
				.AddTo(this);
		}
	}
}