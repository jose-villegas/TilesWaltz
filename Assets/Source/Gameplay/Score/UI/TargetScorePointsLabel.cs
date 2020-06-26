using NaughtyAttributes;
using TilesWalk.Building.Level;
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
	public class TargetScorePointsLabel : ObligatoryComponentBehaviour<TextMeshProUGUI>
	{
		[Inject] private TileViewLevelMap _tileLevelMap;

		[SerializeField] private bool _useSlidingNumber;
		[SerializeField, ShowIf("_useSlidingNumber")] private float _animationSpeed;

		private SlidingNumber _slidingNumber;

		private void Awake()
		{
			if (_useSlidingNumber)
			{
				_slidingNumber = gameObject.AddComponent<SlidingNumber>();
			}

			_tileLevelMap
				.OnLevelMapLoadedAsObservable()
				.Subscribe(
					tileMap =>
					{
						if (_useSlidingNumber)
						{
							Component.text = 0.Localize();
							_slidingNumber.Target(tileMap.Target);
							return;
						}

						Component.text = tileMap.Target.Localize();
					})
				.AddTo(this);
		}
	}
}