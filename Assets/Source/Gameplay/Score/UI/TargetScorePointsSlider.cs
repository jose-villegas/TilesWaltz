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
					}).AddTo(this);
		}
	}
}