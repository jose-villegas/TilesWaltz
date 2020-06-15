using System;
using TilesWalk.Building.Level;
using TilesWalk.General.Patterns;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Level
{
	[RequireComponent(typeof(Camera))]
	public class MapSizeCameraAdapter : ObligatoryComponentBehaviour<Camera>
	{
		[Inject] private TileViewLevelMap _tileViewLevelMap;
		private float _originalSize;

		private void Start()
		{
			_originalSize = Component.orthographicSize;
			_tileViewLevelMap.OnLevelMapLoadedAsObservable().Subscribe(OnLevelMapLoaded).AddTo(this);
		}

		private void OnLevelMapLoaded(LevelMap map)
		{
			Component.orthographicSize = _originalSize + map.MapSize;
		}
	}
}
