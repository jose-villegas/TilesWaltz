using System;
using System.Collections.Generic;
using TilesWalk.Building.Map;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Condition;
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
		[Inject] private TileViewMap _tileMap;
		[Inject] private LevelFinishTracker _levelFinishTracker;

		private void Start()
		{
			_tileMap
				.OnTileMapLoadedAsObservable()
				.Subscribe(
					OnTileMapLoaded
				)
				.AddTo(this);
		}

		private void OnTileMapLoaded(TileMap tileMap)
		{
			if (tileMap.FinishCondition != FinishCondition.TimeLimit)
			{
				transform.parent.gameObject.SetActive(false);
				return;
			}

			var condition = _levelFinishTracker.TimeFinishCondition;
			var start = DateTime.Now;
			var end = DateTime.Now + TimeSpan.FromSeconds(condition.Limit);

			transform.UpdateAsObservable().SubscribeToText(Component, _ =>
			{
				var currentTime = new DateTime((DateTime.Now - start).Ticks).ToString("mm:ss");
				var limitTime = new DateTime((end - start).Ticks).ToString("mm:ss");
				return $"{currentTime}/{limitTime}";
			}).AddTo(this);
		}
	}
}