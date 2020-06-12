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
		[Inject] private List<LevelMap> _availableMaps;

		public ReactiveProperty<string> LevelName { get; } = new ReactiveProperty<string>();
		public LevelMap LevelMap { get; private set; }

		private Subject<LevelMap> _onTileMapFound;

		protected virtual void Start()
		{
			LevelName.Subscribe(level =>
			{
				if (string.IsNullOrEmpty(level)) return;

				LevelMap =  _availableMaps.Find(x => x.Id == level);

				if (LevelMap != null) _onTileMapFound?.OnNext(LevelMap);

			}).AddTo(this);
		}

		public IObservable<LevelMap> OnTileMapFoundAsObservable()
		{
			return _onTileMapFound = _onTileMapFound ?? new Subject<LevelMap>();
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			_onTileMapFound?.OnCompleted();
		}
	}
}