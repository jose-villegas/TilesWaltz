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
		protected Subject<Tile> _onTileClicked;

		protected override void RaiseOnCompletedOnDestroy()
		{
			_onTileRemoved?.OnCompleted();
			_onComboRemoval?.OnCompleted();
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
	}
}