using System;
using System.Collections.Generic;
using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Condition;
using TilesWalk.General;
using TilesWalk.General.Patterns;
using TilesWalk.Navigation.UI;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Limits.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class TimeTrackerLabel : ObligatoryComponentBehaviour<TextMeshProUGUI>
	{
		[Inject] private TileViewLevelMap _tileLevelMap;
		[Inject] private LevelFinishTracker _levelFinishTracker;

		private void Start()
		{
			_tileLevelMap
				.OnLevelMapLoadedAsObservable()
				.Subscribe(
					OnLevelMapLoaded
				)
				.AddTo(this);
		}

		private void OnLevelMapLoaded(LevelMap levelMap)
		{
			if (levelMap.FinishCondition != FinishCondition.TimeLimit && levelMap.Id != Constants.CustomLevelName)
			{
				transform.parent.gameObject.SetActive(false);
				return;
			}

			var condition = _levelFinishTracker.TimeFinishCondition;
			var start = DateTime.Now;
			var end = DateTime.Now + TimeSpan.FromSeconds(condition.Limit);

			transform.UpdateAsObservable().SubscribeToText(Component, _ =>
			{
				var current = new DateTime((DateTime.Now - start).Ticks);
				var limit = new DateTime((end - start).Ticks);
				var currentTime = current.ToString("mm:ss");
				var limitTime = limit.ToString("mm:ss");

				return current < limit ? $"{currentTime}/{limitTime}" : $"{limitTime}/{limitTime}";
			}).AddTo(this);
		}
	}
}