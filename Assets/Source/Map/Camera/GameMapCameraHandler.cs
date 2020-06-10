using System;
using System.Linq;
using TilesWalk.Map.Tile;
using TilesWalk.Navigation.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Map.Camera
{
	public class GameMapCameraHandler : MonoBehaviour
	{
		[Inject] private LevelTilesHandler _levelTilesHandler;
		[Inject] private TileMapDetailsCanvas _detailsCanvas;

		private Vector3 _initialPosition;

		private void Awake()
		{
			_initialPosition = transform.position;

			_detailsCanvas.OnShowAsObservable().Subscribe(OnDetailsCanvasShown).AddTo(this);
			_levelTilesHandler.OnLevelTilesMapsReadyAsObservable().Subscribe(OnLevelTilesMapsReady).AddTo(this);
		}

		private void OnDetailsCanvasShown(Unit unit)
		{
			var levelTile = _levelTilesHandler.LevelTiles.FirstOrDefault(x =>
			{
				if (x.TileMap != null && _detailsCanvas.TileMap != null)
				{
					return x.TileMap.Id == _detailsCanvas.TileMap.Id;
				}

				return false;
			});

			if (levelTile != null) LookAtLevelTile(levelTile);
		}

		private void OnLevelTilesMapsReady(LevelTile[] levelTiles)
		{
			foreach (var tile in levelTiles)
			{
				tile.OnLevelTileClickAsObservable().Subscribe(LookAtLevelTile).AddTo(this);
			}
		}

		private void LookAtLevelTile(LevelTile tile)
		{
			transform.position = _initialPosition + tile.transform.position;
		}
	}
}