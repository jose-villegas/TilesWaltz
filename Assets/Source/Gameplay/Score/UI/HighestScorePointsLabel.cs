using NaughtyAttributes;
using TilesWalk.Extensions;
using TilesWalk.General.Patterns;
using TilesWalk.General.UI;
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

		[SerializeField] private bool _useSlidingNumber;
		[SerializeField, ShowIf("_useSlidingNumber")] private float _animationSpeed;

		private SlidingNumber _slidingNumber;
		private int _initialScore;

		private void Awake()
		{
			if (_useSlidingNumber)
			{
				_slidingNumber = gameObject.AddComponent<SlidingNumber>();
			}

			_levelScorePointsTracker
				.OnScoresLoadedAsObservable()
				.Subscribe(
					score =>
					{
						_initialScore = score.Points.Highest;
						Component.text = score.Points.Highest.Localize();
					})
				.AddTo(this);

			_levelScorePointsTracker
				.OnScorePointsUpdatedAsObservable()
				.SubscribeToText(Component, score =>
				{
					if (score.Points.Last > _initialScore)
					{
						Component.fontStyle = FontStyles.Bold;
					}

					if (_useSlidingNumber)
					{
						_slidingNumber.Target(score.Points.Highest);
						return Component.text;
					}

					return score.Points.Highest.Localize();
				})
				.AddTo(this);
		}
	}
}