using System.Collections.Generic;
using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Condition;
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
	public class LevelMovesConditionLabel : ObligatoryComponentBehaviour<TextMeshProUGUI>, ILevelNameRequire
	{
		[Inject] private MapProviderSolver _solver;

		public ReactiveProperty<string> Name { get; } = new ReactiveProperty<string>();
		public ReactiveProperty<LevelMap> Map { get; } = new ReactiveProperty<LevelMap>();

		private void Awake()
		{
			_solver.InstanceProvider(gameObject);
			Map.SubscribeToText(Component, GetMoves).AddTo(this);
		}

		private string GetMoves(LevelMap map)
		{
			if (map == null) return string.Empty;

			var condition = _solver.Provider.Collection.MovesFinishConditions.Find(x => x.Id == map.Id);

			if (condition != null)
			{
				return condition.Limit.Localize();
			}

			return string.Empty;
		}
	}
}