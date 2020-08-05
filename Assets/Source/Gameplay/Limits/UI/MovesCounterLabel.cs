using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Condition;
using TilesWalk.Gameplay.Score;
using TilesWalk.General.Patterns;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Limits.UI
{
	/// <summary>
	/// A text label that tracks the amount of movements left for the player
	/// in a level
	/// </summary>
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class MovesCounterLabel : ObligatoryComponentBehaviour<TextMeshProUGUI>
	{
		[Inject] private TileViewLevelMap _tileLevelMap;
		[Inject] private LevelFinishTracker _levelFinishTracker;

		private void Awake()
		{
			_levelFinishTracker.OnTrackersSetupFinishAsObservable().Subscribe(OnLevelMapLoaded).AddTo(this);
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