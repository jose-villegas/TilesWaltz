using TilesWalk.Building.Level;
using TilesWalk.Navigation.UI;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Level.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class LevelTileMapNameLabel : ObligatoryComponentBehaviour<TextMeshProUGUI>
	{
		[Inject] private TileViewMap _tileViewMap;

		private void Start()
		{
			if (_tileViewMap.TileMap != null && !string.IsNullOrEmpty(_tileViewMap.TileMap.Id))
			{
				Component.text = _tileViewMap.TileMap.Id;
			}

			_tileViewMap.OnTileMapLoadedAsObservable().Subscribe(OnTileMapLoaded).AddTo(this);
		}

		private void OnTileMapLoaded(TileMap tileMap)
		{
			Component.text = tileMap.Id;
		}
	}
}