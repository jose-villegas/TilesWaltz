using System;
using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Condition;
using TilesWalk.Gameplay.Score;
using TilesWalk.General.Patterns;
using TilesWalk.General.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TilesWalk.Gameplay.Level.UI
{
	[RequireComponent(typeof(Animator))]
	public class CongratsAnimationCanvas : CanvasGroupBehaviour
	{
		[Inject] private LevelFinishTracker _levelFinishTracker;
		[Inject] private GameScoresHelper _gameScoresHelper;
		[Inject] private TileViewLevelMap _levelMap;

		[SerializeField] private SlidingNumber _slidingNumber;
		[SerializeField] private Slider _slider;
		[SerializeField] private Button _retry;
		[SerializeField] private Button _summary;

		private Animator _animator;

		// Start is called before the first frame update
		void Start()
		{
			_animator = GetComponent<Animator>();
			_levelFinishTracker.OnLevelFinishAsObservable().Subscribe(OnLevelFinish).AddTo(this);
		}

		private void OnLevelFinish(LevelScore score)
		{
			_slider.maxValue = _levelMap.LevelMap.Target;
			// initialize number at 0
			_slidingNumber.Component.text = 0.Localize();

			// update slider with number
			_slidingNumber.ObserveEveryValueChanged(x => x.Current).Subscribe(value =>
			{
				if (value <= _levelMap.LevelMap.Target)
				{
					var starCount = _gameScoresHelper.GetStarCount(_levelMap.LevelMap, (int)value);

					if (starCount >= 1)
					{
						_animator.SetInteger("Stars", starCount);
					}

					_slider.value = value;
				}
			}).AddTo(this);

			_slidingNumber.Target(Mathf.Min(score.Points.Last, _slider.maxValue));

			Show();
		}
	}
}