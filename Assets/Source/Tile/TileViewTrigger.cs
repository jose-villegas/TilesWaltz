using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;

namespace TilesWalk.Tile
{
	public class TileViewTrigger : ObservableTriggerBase
	{
		protected Subject<List<Tile>> _onTileRemoved;
		protected Subject<List<Tile>> _onComboRemoval;
		protected Subject<Tuple<List<Tile>, TilePowerUp>> _onPowerUpRemoval;
		protected Subject<Tile> _onTileClicked;

		protected override void RaiseOnCompletedOnDestroy()
		{
			_onTileRemoved?.OnCompleted();
			_onComboRemoval?.OnCompleted();
			_onPowerUpRemoval?.OnCompleted();
			_onTileClicked?.OnCompleted();
		}

		public IObservable<List<Tile>> OnTileRemovedAsObservable()
		{
			return _onTileRemoved = _onTileRemoved ?? new Subject<List<Tile>>();
		}

		public IObservable<List<Tile>> OnComboRemovalAsObservable()
		{
			return _onComboRemoval = _onComboRemoval ?? new Subject<List<Tile>>();
		}

		public IObservable<Tile> OnTileClickedAsObservable()
		{
			return _onTileClicked = _onTileClicked ?? new Subject<Tile>();
		}

		public IObservable<Tuple<List<Tile>, TilePowerUp>> OnPowerUpRemovalAsObservable()
		{
			return _onPowerUpRemoval = _onPowerUpRemoval ?? new Subject<Tuple<List<Tile>, TilePowerUp>>();
		}
	}
}