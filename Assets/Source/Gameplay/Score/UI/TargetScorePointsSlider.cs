using System.Collections.Generic;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Condition;
using TilesWalk.General.Patterns;
using TilesWalk.Navigation.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TilesWalk.Gameplay.Score.UI
{
	[RequireComponent(typeof(Slider))]
	public class TargetScorePointsSlider : ObligatoryComponentBehaviour<Slider>
	{
		[Inject] private TileViewLevelMap _tileLevelMap;
		[Inject] private LevelScorePointsTracker _levelScorePointsTracker;
		[Inject] private GameScoresHelper _gameScoresHelper;

		[SerializeField] private Sprite _starEmpty;
		[SerializeField] private Sprite _starFull;
		[SerializeField] private List<Image> _stars;

		private void Start()
		{
			_levelScorePointsTracker
				.OnScorePointsUpdatedAsObservable()
				.Subscribe(
					score =>
					{
						float last = score.Points.Last;
						float ceil = _tileLevelMap.LevelMap.Target;
						Component.value = Mathf.Min(1f, last / ceil);

						var starCount = _gameScoresHelper.GetStarCount(_tileLevelMap.LevelMap, score.Points.Last);
						_stars.ForEach(i => i.sprite = _starEmpty);

						for (int i = 0; i < starCount && i < _stars.Count; i++)
						{
							_stars[i].sprite = _starFull;
						}
					}).AddTo(this);
		}
	}
}