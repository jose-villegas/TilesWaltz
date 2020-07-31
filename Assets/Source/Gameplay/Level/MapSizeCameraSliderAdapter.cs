using TilesWalk.Building.Level;
using TilesWalk.General.Display;
using TilesWalk.General.Patterns;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TilesWalk.Gameplay.Level
{
	[RequireComponent(typeof(Camera))]
	public class MapSizeCameraSliderAdapter : ObligatoryComponentBehaviour<Camera>
	{
		[SerializeField] private Slider _mapSizeSlider;

		[Inject] private TileViewLevelMap _tileLevelMap;
        [Inject] private GameDisplayConfiguration _displayConfiguration;

		private float _originalSize;

		private void Awake()
		{
			_tileLevelMap.OnLevelMapDataLoadedAsObservable().Subscribe(OnLevelMapLoaded).AddTo(this);

			if (_mapSizeSlider != null)
			{
				_mapSizeSlider.OnValueChangedAsObservable().Subscribe(val =>
				{
					Component.orthographicSize = _displayConfiguration.GetOrthogonalSize((int)val);
				}).AddTo(this);
			}
		}

		private void OnLevelMapLoaded(LevelMap map)
		{
			Component.orthographicSize = _displayConfiguration.GetOrthogonalSize(map.MapSize);
		}
	}
}