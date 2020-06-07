using TilesWalk.Extensions;
using TilesWalk.Navigation.UI;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Score.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class HighestScorePointsLabel : ObligatoryComponentBehaviour<TextMeshProUGUI>
	{
		[Inject] private LevelScorePointsTracker _levelScorePointsTracker;

		private void Awake()
		{
			_levelScorePointsTracker
				.OnScoresLoadedAsObservable()
				.Subscribe(
					score => Component.text = score.Points.Highest.Localize()
				)
				.AddTo(this);

			_levelScorePointsTracker
				.OnScorePointsUpdatedAsObservable()
				.SubscribeToText(Component, score => score.Points.Highest.Localize())
				.AddTo(this);
		}
	}
}