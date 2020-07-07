using TilesWalk.Building.Level;
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
		private float _originalSize;

		private void Awake()
		{
			_originalSize = Component.orthographicSize;
			_tileLevelMap.OnLevelMapLoadedAsObservable().Subscribe(OnLevelMapLoaded).AddTo(this);

			if (_mapSizeSlider != null)
			{
				_mapSizeSlider.OnValueChangedAsObservable().Subscribe(val =>
				{
					Component.orthographicSize = _originalSize + val;
				}).AddTo(this);
			}
		}

		private void OnLevelMapLoaded(LevelMap map)
		{
			Component.orthographicSize = _originalSize + map.MapSize;
		}
	}
}