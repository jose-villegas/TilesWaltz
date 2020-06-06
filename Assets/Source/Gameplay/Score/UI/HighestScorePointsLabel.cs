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
		[Inject] private LevelScoreTracker _levelScoreTracker;

		private void Awake()
		{
			_levelScoreTracker
				.OnScoresLoadedAsObservable()
				.Subscribe(
					score => Component.text = score.Points.Highest.ToString()
				)
				.AddTo(this);

			_levelScoreTracker
				.OnScorePointsUpdatedAsObservable()
				.SubscribeToText(Component, score => score.Points.Highest.ToString())
				.AddTo(this);
		}
	}
}