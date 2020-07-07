using System;
using System.Collections.Generic;
using TilesWalk.Gameplay.Display;
using UniRx;
using UniRx.Triggers;

namespace TilesWalk.Tile.Level
{
	public abstract class LevelTileViewTriggerBase : ObservableTriggerBase
	{
		public Subject<List<Tile>> OnTileRemoved;
		public Subject<List<Tile>> OnComboRemoval;
		public Subject<Tuple<List<Tile>, TilePowerUp>> OnPowerUpRemoval;
		public Subject<Tile> OnTileClicked;

		protected override void RaiseOnCompletedOnDestroy()
		{
			OnTileRemoved?.OnCompleted();
			OnComboRemoval?.OnCompleted();
			OnPowerUpRemoval?.OnCompleted();
			OnTileClicked?.OnCompleted();
		}

		public IObservable<List<Tile>> OnTileRemovedAsObservable()
		{
			return OnTileRemoved = OnTileRemoved ?? new Subject<List<Tile>>();
		}

		public IObservable<List<Tile>> OnComboRemovalAsObservable()
		{
			return OnComboRemoval = OnComboRemoval ?? new Subject<List<Tile>>();
		}

		public IObservable<Tile> OnTileClickedAsObservable()
		{
			return OnTileClicked = OnTileClicked ?? new Subject<Tile>();
		}

		public IObservable<Tuple<List<Tile>, TilePowerUp>> OnPowerUpRemovalAsObservable()
		{
			return OnPowerUpRemoval = OnPowerUpRemoval ?? new Subject<Tuple<List<Tile>, TilePowerUp>>();
		}
	}
}