using System.Collections.Generic;
using System.Linq;
using BayatGames.SaveGameFree.Examples;
using TilesWalk.Building.Level;
using TilesWalk.Navigation.UI;
using TMPro.EditorUtilities;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace TilesWalk.Navigation.Map
{
	public class ShowTileMapDetailsCanvas : LevelNameRequireBehaviour
	{
		[Inject] private TileMapDetailsCanvas _detailsCanvas;
		[Inject] private MapLevelBridge _mapLevelBridge;
		[Inject] private List<TileMap> _availableMaps;

		private void Awake()
		{
			transform.OnMouseDownAsObservable().Subscribe(OnMapTileClick).AddTo(this);
		}

		protected override void OnTileMapFound()
		{
			// check if there is any completed tile map on the bridge
			if (_mapLevelBridge.Results != null)
			{
				// the results are for this level
				if (_mapLevelBridge.Results.Id == TileMap.Id)
				{
					// if failed to reach all stars show this map again
					if (_mapLevelBridge.Results.Points.Last < TileMap.Target)
					{
						_detailsCanvas.LevelName.Value = LevelName.Value;
						_detailsCanvas.Show();
					}
				}
				else
				{
					if (_mapLevelBridge.Results.Points.Last >= TileMap.Target)
					{
						// find next level
						var index = _availableMaps.IndexOf(TileMap);
						var fromIndex = _availableMaps.GetRange(index, _availableMaps.Count - index);
						// ignore map as it should have a target of 0
						var found = fromIndex.First(x => x.Target > 0 && x.Id != TileMap.Id);

						if (found != null)
						{
							_detailsCanvas.LevelName.Value = found.Id;
							_detailsCanvas.Show();
						}
					}
				}
			}
		}

		private void OnMapTileClick(Unit u)
		{
			if (_detailsCanvas.IsVisible && _detailsCanvas.LevelName.Value == TileMap.Id)
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