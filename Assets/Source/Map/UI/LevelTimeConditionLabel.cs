using System;
using System.Collections.Generic;
using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Condition;
using TilesWalk.General.Patterns;
using TilesWalk.Map.Scaffolding;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Map.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class LevelTimeConditionLabel : ObligatoryComponentBehaviour<TextMeshProUGUI>, ILevelNameRequire
	{
		[Inject] private List<TimeFinishCondition> _timeFinishConditions;

		public ReactiveProperty<string> Name { get; } = new ReactiveProperty<string>();
		public ReactiveProperty<LevelMap> Map { get; } = new ReactiveProperty<LevelMap>();

		private void Awake()
		{
			Map.SubscribeToText(Component, GetTime).AddTo(this);
		}

		private string GetTime(LevelMap map)
		{
			if (map == null) return string.Empty;

			var condition = _timeFinishConditions.Find(x => x.Id == map.Id);

			if (condition != null)
			{
				var time = new DateTime(TimeSpan.FromSeconds(condition.Limit).Ticks);
				return time.ToString("mm:ss");
			}

			return string.Empty;
		}
	}
}