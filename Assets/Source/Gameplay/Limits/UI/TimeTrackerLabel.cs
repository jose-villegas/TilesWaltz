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
		[Inject] private List<TimeFinishCondition> _timeFinishConditions;
		private DateTime _start;
		private TimeSpan _limit;

		private void Start()
		{
			_tileMap
				.OnTileMapLoadedAsObservable()
				.Subscribe(
					OnTileMapLoaded
				)
				.AddTo(this);

			transform.UpdateAsObservable().SubscribeToText(Component, _ =>
			{
				var currentTime = new DateTime((DateTime.Now - _start).Ticks).ToString("mm:ss");
				var limitTime = new DateTime(_limit.Ticks).ToString("mm:ss");
				return string.Format("{0}/{1}", currentTime, limitTime);
			}).AddTo(this);
		}

		private void OnTileMapLoaded(TileMap tileMap)
		{
			_start = DateTime.Now;

			if (tileMap.FinishCondition != FinishCondition.TimeLimit &&
			    tileMap.FinishCondition != FinishCondition.TimeAndMoveLimit)
			{
				transform.parent.gameObject.SetActive(false);
				return;
			}

			var condition = _timeFinishConditions.Find(x => x.Id == tileMap.Id);

			if (condition != null)
			{
				_limit = TimeSpan.FromSeconds(condition.Limit);
			}
		}
	}
}