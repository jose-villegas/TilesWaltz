using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TilesWalk.Gameplay.Animation;
using TilesWalk.General.Camera;
using TilesWalk.General.Patterns;
using TilesWalk.Map.Tile;
using TilesWalk.Navigation.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Map.Camera
{
	/// <summary>
	/// Camera controller for the game map view
	/// </summary>
	[RequireComponent(typeof(CameraDrag))]
	public class GameMapCameraHandler : ObligatoryComponentBehaviour<CameraDrag>
	{
		[Inject] private LevelTilesHandler _levelTilesHandler;
		[Inject] private LevelMapDetailsCanvas _detailsCanvas;
		[Inject] private AnimationConfiguration _animationConfiguration;

		private Vector3 _initialPosition;
		private IDisposable _lookAt;

		private void Awake()
		{
			_initialPosition = transform.position;

			_detailsCanvas.OnShowAsObservable().Subscribe(OnDetailsCanvasShown).AddTo(this);
			_detailsCanvas.OnHideAsObservable().Subscribe(OnDetailsCanvasHiden).AddTo(this);
			_levelTilesHandler.OnLevelTilesMapsReadyAsObservable().Subscribe(OnLevelTilesMapsReady).AddTo(this);
		}

		private void OnDetailsCanvasHiden(Unit u)
		{
			Component.enabled = true;
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

			Component.enabled = false;

			if (levelTile != null)
			{
				LookAtLevelTile(levelTile);
			}
		}


		/// <summary>
		/// Subscribes tile click action
		/// </summary>
		/// <param name="levelTiles"></param>
		private void OnLevelTilesMapsReady(List<LevelTile> levelTiles)
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
			_lookAt?.Dispose();
			_lookAt = GoToPosition(_initialPosition + tile.transform.position).ToObservable().Subscribe().AddTo(this);
		}

		private IEnumerator GoToPosition(Vector3 position)
		{
			var initial = transform.position;
			var t = 0f;

			while (t <= _animationConfiguration.TargetTileTime)
			{
				transform.position = Vector3.Lerp(initial, position, t / _animationConfiguration.TargetTileTime);
				t += Time.deltaTime;
				yield return null;
			}

			transform.position = position;
		}
	}
}