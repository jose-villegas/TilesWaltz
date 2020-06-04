using System.Collections.Generic;
using TilesWalk.Building.Map;
using TilesWalk.Navigation.UI;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace TilesWalk.Navigation.Map
{
	public class ShowTileMapDetails : MonoBehaviour
	{
		[SerializeField] private string _levelName;

		[Inject] private TileMapDetails _details;
		[Inject] private List<TileMap> _availableMaps;
		private TileMap _tileMap;

		private void Start()
		{
			_tileMap = _availableMaps.Find(x => x.Id == _levelName);
			transform.OnMouseDownAsObservable().Subscribe(OnMapTileClick).AddTo(this);
		}

		private void OnMapTileClick(Unit u)
		{
			if (_details.IsVisible)
			{
				_details.Hide();
			}
			else
			{
				_details.LoadMapData(_tileMap);
				_details.Show();
			}
		}
	}
}