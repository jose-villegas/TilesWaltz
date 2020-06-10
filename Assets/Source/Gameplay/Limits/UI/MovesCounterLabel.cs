using System.Collections.Generic;
using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Condition;
using TilesWalk.General.Patterns;
using TilesWalk.Navigation.UI;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Limits.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class MovesCounterLabel : ObligatoryComponentBehaviour<TextMeshProUGUI>
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

			_tileMap
				.OnTileRemovedAsObservable()
				.SubscribeToText(Component, _ =>
				{
					var condition = _levelFinishTracker.MovesFinishCondition;
					return string.Format("%02d/%02d", condition.Tracker, condition.Limit);
				})
				.AddTo(this);
		}

		private void OnTileMapLoaded(TileMap tileMap)
		{
			if (tileMap.FinishCondition != FinishCondition.MovesLimit)
			{
				transform.parent.gameObject.SetActive(false);
				return;
			}

			var condition = _levelFinishTracker.MovesFinishCondition;

			Component.text = string.Format("%02d/%02d", condition.Tracker, condition.Limit);
		}
	}
}