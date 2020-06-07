using System.Collections.Generic;
using TilesWalk.Building.Level;
using TilesWalk.Navigation.UI;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace TilesWalk.Navigation.Map
{
	public class ShowTileMapDetailsCanvas : MonoBehaviour
	{
		[SerializeField] private string _levelName;

		[Inject] private TileMapDetailsCanvas _detailsCanvas;
		[Inject] private List<TileMap> _availableMaps;
		private TileMap _tileMap;

		private void Start()
		{
			_tileMap = _availableMaps.Find(x => x.Id == _levelName);
			transform.OnMouseDownAsObservable().Subscribe(OnMapTileClick).AddTo(this);
		}

		private void OnMapTileClick(Unit u)
		{
			if (_detailsCanvas.IsVisible)
			{
				_detailsCanvas.Hide();
			}
			else
			{
				_detailsCanvas.LoadMapData(_tileMap);
				_detailsCanvas.Show();
			}
		}
	}
}