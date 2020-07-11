using System;
using System.Collections.Generic;
using TilesWalk.Building.Level;
using TilesWalk.General.Display;
using TilesWalk.General.Patterns;
using TilesWalk.Map.Tile;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TilesWalk.Map.Camera
{
    public class GameMapSizeSliderCameraAdapter : ObligatoryComponentBehaviour<UnityEngine.Camera>
    {
		[SerializeField] private Slider _mapSizeSlider;

        [Inject] private GameLevelTilesInitializer _gameLevelTilesInitializer;
        [Inject] private GameDisplayConfiguration _displayConfiguration;

        private float _originalSize;

        private void Awake()
        {
            _gameLevelTilesInitializer.OnLevelTilesMapsReadyAsObservable().Subscribe(OnLevelMapLoaded).AddTo(this);

            if (_mapSizeSlider != null)
            {
                _mapSizeSlider.OnValueChangedAsObservable().Subscribe(val =>
                {
                    Component.orthographicSize = _displayConfiguration.GetOrthogonalSize((int)val);
                }).AddTo(this);
            }
        }

        private void OnLevelMapLoaded(List<GameLevelTile> obj)
        {
            Component.orthographicSize = _displayConfiguration.GetOrthogonalSize(3);
        }
	}
}