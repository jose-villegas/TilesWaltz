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
		[Inject] private LevelScoreTracker _levelScoreTracker;

		private void Awake()
		{
			_levelScoreTracker
				.OnScorePointsUpdatedAsObservable()
				.SubscribeToText(Component, score => score.Points.Last.ToString())
				.AddTo(this);
		}
	}
}