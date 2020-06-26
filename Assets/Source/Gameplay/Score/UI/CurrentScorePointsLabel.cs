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
	public class CurrentScorePointsLabel : ObligatoryComponentBehaviour<TextMeshProUGUI>
	{
		[Inject] private LevelScorePointsTracker _levelScorePointsTracker;

		[SerializeField] private bool _useSlidingNumber;
		[SerializeField, ShowIf("_useSlidingNumber")] private float _animationSpeed;

		private SlidingNumber _slidingNumber;

		private void Awake()
		{
			Component.text = 0.Localize();

			if (_useSlidingNumber)
			{
				_slidingNumber = gameObject.AddComponent<SlidingNumber>();
			}

			_levelScorePointsTracker
				.OnScorePointsUpdatedAsObservable()
				.SubscribeToText(Component, score =>
				{
					if (_useSlidingNumber)
					{
						_slidingNumber.Target(score.Points.Last);
						return Component.text;
					}

					return score.Points.Last.Localize();
				})
				.AddTo(this);
		}
	}
}