using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.General.Patterns;
using TilesWalk.Map.Scaffolding;
using TMPro;
using UniRx;
using UnityEngine;

namespace TilesWalk.Map.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class LevelTargetPointsLabel : ObligatoryComponentBehaviour<TextMeshProUGUI>, ILevelNameRequire
	{
		public ReactiveProperty<string> Name { get; } = new ReactiveProperty<string>();
		public ReactiveProperty<LevelMap> Map { get; } = new ReactiveProperty<LevelMap>();

		private void Awake()
		{
			Map.SubscribeToText(Component, map => map != null ? map.Target.Localize() : "").AddTo(this);
		}
	}
}