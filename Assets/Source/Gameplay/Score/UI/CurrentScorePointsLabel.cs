using TilesWalk.Extensions;
using TilesWalk.Navigation.UI;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Score.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class CurrentScorePointsLabel : ObligatoryComponentBehaviour<TextMeshProUGUI>
	{
		[Inject] private LevelScorePointsTracker _levelScorePointsTracker;

		private void Awake()
		{
			_levelScorePointsTracker
				.OnScorePointsUpdatedAsObservable()
				.SubscribeToText(Component, score => score.Points.Last.Localize())
				.AddTo(this);
		}
	}
}