using TilesWalk.Extensions;
using TilesWalk.Navigation.UI;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Score.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class CurrentScoreLabel : ObligatoryComponentBehaviour<TextMeshProUGUI>
	{
		[Inject] private ScoreTracker _scoreTracker;

		private void Awake()
		{
			_scoreTracker
				.OnScoreUpdatedAsObservable()
				.SubscribeToText(Component, score => score.LastScore.ToString())
				.AddTo(this);
		}
	}
}