using System.Collections.Generic;
using System.Linq;
using BayatGames.SaveGameFree.Examples;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Score;
using TilesWalk.Navigation.UI;
using TMPro.EditorUtilities;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace TilesWalk.Navigation.Map
{
	public class LevelTile : LevelNameRequireBehaviour
	{
		[Inject] private TileMapDetailsCanvas _detailsCanvas;
		[Inject] private MapLevelBridge _mapLevelBridge;


		private void Awake()
		{
			transform.OnMouseDownAsObservable().Subscribe(OnMapTileClick).AddTo(this);
		}

		private void OnMapTileClick(Unit u)
		{
			if (_detailsCanvas.IsVisible && _detailsCanvas.LevelName.Value == LevelName.Value)
			{
				_detailsCanvas.Hide();
			}
			else
			{
				_detailsCanvas.LevelName.Value = LevelName.Value;
				_detailsCanvas.Show();
			}
		}
	}
}