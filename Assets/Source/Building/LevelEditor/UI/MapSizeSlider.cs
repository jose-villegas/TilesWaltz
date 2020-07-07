using TilesWalk.Building.Level;
using TilesWalk.General.Patterns;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TilesWalk.Building.LevelEditor.UI
{
	[RequireComponent(typeof(Slider))]
	public class MapSizeSlider : ObligatoryComponentBehaviour<Slider>
	{
		[Inject] private TileViewLevelMap _tileViewLevelMap;

		private void Awake()
		{
			_tileViewLevelMap.OnLevelMapLoadedAsObservable().Subscribe(OnLevelMapLoaded).AddTo(this);
		}

		private void OnLevelMapLoaded(LevelMap map)
		{
			Component.value = map.MapSize;

			if (Component != null)
			{
				Component.OnValueChangedAsObservable().Subscribe(val =>
				{
					_tileViewLevelMap.Map.MapSize = (int)val;
				}).AddTo(this);
			}
		}
	}
}