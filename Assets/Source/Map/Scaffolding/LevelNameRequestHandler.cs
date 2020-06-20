using System;
using TilesWalk.Building.Level;
using TilesWalk.Map.General;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Provider = TilesWalk.Map.General.Provider;

namespace TilesWalk.Map.Scaffolding
{
	[RequireComponent(typeof(IMapProvider))]
	public class LevelNameRequestHandler : ObservableTriggerBase
	{
		[SerializeField] private MapProviderSolver _solver;
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
			if (_solver == null) _solver = new MapProviderSolver(gameObject);

			_solver.InstanceProvider();

			ScaffoldRequiredNames(RawName);
			Name.Subscribe(level =>
			{
				if (string.IsNullOrEmpty(level)) return;

				Map = _solver.Provider.AvailableMaps.Find(x => x.Id == level);

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