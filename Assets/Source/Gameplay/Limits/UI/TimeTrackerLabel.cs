﻿using System;
using System.Collections.Generic;
using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Condition;
using TilesWalk.Gameplay.Score;
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

		private void Awake()
		{
			_tileLevelMap
				.OnLevelMapLoadedAsObservable()
				.Subscribe(map =>
				{
					if (map.FinishCondition != FinishCondition.TimeLimit) return;

					_levelFinishTracker.OnTrackersSetupFinishAsObservable().Subscribe(OnLevelMapLoaded).AddTo(this);
				}).AddTo(this);
		}

		private void OnLevelMapLoaded(LevelScore score)
		{
			try
			{
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
			catch (Exception e)
			{
				Console.WriteLine(e);
				gameObject.SetActive(false);
			}
		}
	}
}