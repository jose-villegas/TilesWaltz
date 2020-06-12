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
		[Inject] private TileViewMap _tileViewMap;

		private void Start()
		{
			_tileViewMap.OnTileMapLoadedAsObservable().Subscribe(OnTileMapLoaded).AddTo(this);
		}

		private void OnTileMapLoaded(TileMap map)
		{
			Component.orthographicSize += map.MapSize;
		}
	}
}
