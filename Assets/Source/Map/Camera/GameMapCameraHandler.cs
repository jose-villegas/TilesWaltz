using System.Linq;
using TilesWalk.Map.Tile;
using TilesWalk.Navigation.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Map.Camera
{
	/// <summary>
	/// Camera ocntroller for the game map view
	/// </summary>
	public class GameMapCameraHandler : MonoBehaviour
	{
		[Inject] private LevelTilesHandler _levelTilesHandler;
		[Inject] private LevelMapDetailsCanvas _detailsCanvas;

		private Vector3 _initialPosition;

		private void Awake()
		{
			_initialPosition = transform.position;

			_detailsCanvas.OnShowAsObservable().Subscribe(OnDetailsCanvasShown).AddTo(this);
			_levelTilesHandler.OnLevelTilesMapsReadyAsObservable().Subscribe(OnLevelTilesMapsReady).AddTo(this);
		}

		/// <summary>
		/// Find the selected <see cref="LevelTile"/> to focus the camera at
		/// </summary>
		/// <param name="unit"></param>
		private void OnDetailsCanvasShown(Unit unit)
		{
			var levelTile = _levelTilesHandler.LevelTiles.FirstOrDefault(x =>
			{
				if (x.Map != null && _detailsCanvas.LevelRequest.Map != null)
				{
					return x.Map.Value.Id == _detailsCanvas.LevelRequest.Map.Id;
				}

				return false;
			});

			if (levelTile != null) LookAtLevelTile(levelTile);
		}


		/// <summary>
		/// Subscribes tile click action
		/// </summary>
		/// <param name="levelTiles"></param>
		private void OnLevelTilesMapsReady(LevelTile[] levelTiles)
		{
			foreach (var tile in levelTiles)
			{
				tile.OnLevelTileClickAsObservable().Subscribe(LookAtLevelTile).AddTo(this);
			}
		}

		/// <summary>
		/// Translates the camera to a proper position to look at the selected <see cref="LevelTile"/>
		/// </summary>
		/// <param name="tile"></param>
		private void LookAtLevelTile(LevelTile tile)
		{
			transform.position = _initialPosition + tile.transform.position;
		}
	}
}