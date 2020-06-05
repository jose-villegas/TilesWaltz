using TilesWalk.Extensions;
using TilesWalk.Navigation.UI;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Score.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class RecordScoreLabel : ObligatoryComponentBehaviour<TextMeshProUGUI>
	{
		[Inject] private ScoreTracker _scoreTracker;

		private void Awake()
		{
			_scoreTracker
				.OnScoresLoadedAsObservable()
				.Subscribe(
					_ => { },
					() => Component.text = _scoreTracker.ActiveScore.HighestScore.ToString()
				)
				.AddTo(this);

			_scoreTracker
				.OnScoreUpdatedAsObservable()
				.SubscribeToText(Component, score => score.HighestScore.ToString())
				.AddTo(this);
		}
	}
}