using System;
using System.Collections.Generic;
using TilesWalk.Building.Level;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace TilesWalk.Map.Scaffolding
{
	public abstract class LevelNameRequireBehaviour : ObservableTriggerBase, ILevelNameRequire
	{
		[Inject] private List<TileMap> _availableMaps;

		public ReactiveProperty<string> LevelName { get; } = new ReactiveProperty<string>();
		public TileMap TileMap { get; private set; }

		private Subject<TileMap> _onTileMapFound;

		protected virtual void Start()
		{
			LevelName.Subscribe(level =>
			{
				if (string.IsNullOrEmpty(level)) return;

				TileMap =  _availableMaps.Find(x => x.Id == level);

				if (TileMap != null) _onTileMapFound?.OnNext(TileMap);

			}).AddTo(this);
		}

		public IObservable<TileMap> OnTileMapFoundAsObservable()
		{
			return _onTileMapFound = _onTileMapFound ?? new Subject<TileMap>();
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			_onTileMapFound?.OnCompleted();
		}
	}
}