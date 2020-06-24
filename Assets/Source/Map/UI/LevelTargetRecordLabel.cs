using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.General.Patterns;
using TilesWalk.Map.General;
using TilesWalk.Map.Scaffolding;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Map.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class LevelTargetRecordLabel : ObligatoryComponentBehaviour<TextMeshProUGUI>, ILevelNameRequire
	{
		[Inject] private MapProviderSolver _solver;

		public ReactiveProperty<string> Name { get; } = new ReactiveProperty<string>();
		public ReactiveProperty<LevelMap> Map { get; } = new ReactiveProperty<LevelMap>();

		private void Awake()
		{
			_solver.InstanceProvider(gameObject);
			Map.SubscribeToText(Component,
				map => map != null ? _solver.Provider.Records[map.Id].Points.Highest.Localize() : "").AddTo(this);
		}
	}
}