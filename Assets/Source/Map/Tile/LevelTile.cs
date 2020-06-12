using System;
using TilesWalk.Map.Bridge;
using TilesWalk.Map.Scaffolding;
using TilesWalk.Navigation.UI;
using UniRx;
using UniRx.Triggers;
using Zenject;

namespace TilesWalk.Map.Tile
{
	public class LevelTile : LevelNameRequireBehaviour
	{
		[Inject] private LevelMapDetailsCanvas _detailsCanvas;
		[Inject] private MapLevelBridge _mapLevelBridge;

		private Subject<LevelTile> _onLevelTileClick;

		private void OnMouseDown()
		{
			OnMapTileClick();
		}

		public void OnMapTileClick()
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

			_onLevelTileClick?.OnNext(this);
		}

		public IObservable<LevelTile> OnLevelTileClickAsObservable()
		{
			return _onLevelTileClick = _onLevelTileClick ?? new Subject<LevelTile>();
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			_onLevelTileClick?.OnCompleted();
			base.RaiseOnCompletedOnDestroy();
		}
	}
}