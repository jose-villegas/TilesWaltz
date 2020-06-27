using System.Collections.Generic;
using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Condition;
using TilesWalk.Gameplay.Score;
using TilesWalk.General;
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

		private void Awake()
		{
			_tileLevelMap
				.OnLevelMapLoadedAsObservable()
				.Subscribe(map =>
				{
					if (map.FinishCondition != FinishCondition.MovesLimit) return;

					_levelFinishTracker.OnTrackersSetupFinishAsObservable().Subscribe(OnLevelMapLoaded).AddTo(this);
				}).AddTo(this);
		}

		private void OnLevelMapLoaded(LevelScore score)
		{
			var condition = _levelFinishTracker.MovesFinishCondition;

			if (condition == null) return;

			Component.text = $"{condition.Tracker.Value}/{condition.Limit.Localize()}";
			condition.Tracker.SubscribeToText(Component, value => $"{value.Localize()}/{condition.Limit.Localize()}")
				.AddTo(this);
		}
	}
}