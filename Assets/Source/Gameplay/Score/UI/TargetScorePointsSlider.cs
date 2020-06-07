using TilesWalk.Building.Map;
using TilesWalk.Gameplay.Condition;
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
		[Inject] private TileViewMap _tileMap;
		[Inject] private LevelScoreTracker _levelScoreTracker;

		private void Start()
		{
			_levelScoreTracker
				.OnScorePointsUpdatedAsObservable()
				.Subscribe(
					score =>
					{
						float last = score.Points.Last;
						float ceil = _tileMap.TileMap.Target;
						Component.value = Mathf.Min(1f, last / ceil);
					}).AddTo(this);
		}
	}
}