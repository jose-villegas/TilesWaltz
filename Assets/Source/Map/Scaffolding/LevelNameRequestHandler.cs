using System;
using System.Collections.Generic;
using TilesWalk.Building.Level;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace TilesWalk.Map.Scaffolding
{
	public class LevelNameRequestHandler : ObservableTriggerBase
	{
		[Inject] private List<LevelMap> _availableMaps;

		[SerializeField] private string _levelName;

		public ReactiveProperty<string> Name { get; } = new ReactiveProperty<string>();
		public LevelMap Map { get; private set; }

		public string RawName
		{
			get => _levelName;
			set => _levelName = value;
		}

		private Subject<LevelMap> _onTileMapFound;

		private void Awake()
		{
			ScaffoldRequiredNames(RawName);
			Name.Subscribe(level =>
			{
				if (string.IsNullOrEmpty(level)) return;

				Map = _availableMaps.Find(x => x.Id == level);

				if (Map != null)
				{
					ScaffoldRequiredNames(level);
					_onTileMapFound?.OnNext(Map);
				}
			}).AddTo(this);
			Name.Value = RawName;
		}

		protected virtual void ScaffoldRequiredNames(string val)
		{
			var children = transform.GetComponentsInChildren<ILevelNameRequire>();

			foreach (var child in children)
			{
				child.Map.Value = Map;
				child.Name.Value = val;
			}
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