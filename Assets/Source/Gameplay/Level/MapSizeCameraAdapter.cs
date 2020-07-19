using System;
using TilesWalk.Building.Level;
using TilesWalk.General.Display;
using TilesWalk.General.Patterns;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Level
{
    [RequireComponent(typeof(Camera))]
    public class MapSizeCameraAdapter : ObligatoryComponentBehaviour<Camera>
    {
        [Inject] private TileViewLevelMap _tileLevelMap;
        [Inject] private GameDisplayConfiguration _displayConfiguration;

        private void Awake()
        {
            _tileLevelMap.OnLevelMapDataLoadedAsObservable().Subscribe(OnLevelMapLoaded).AddTo(this);
        }

        private void OnLevelMapLoaded(LevelMap map)
        {
            Component.orthographicSize = _displayConfiguration.GetOrthogonalSize(map.MapSize);
        }
    }
}