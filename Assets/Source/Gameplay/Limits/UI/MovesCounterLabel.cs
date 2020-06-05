using System.Collections.Generic;
using TilesWalk.Building.Map;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Condition;
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
		[Inject] private List<MovesFinishCondition> _movesFinishConditions;

		private int _counter;
		private int _limit;

		private void Start()
		{
			_tileMap
				.OnTileMapLoadedAsObservable()
				.Subscribe(
					_ => { },
					OnTileMapLoaded
				)
				.AddTo(this);

			_tileMap
				.OnTileRemovedAsObservable()
				.SubscribeToText(Component, _ =>
				{
					_counter++;
					return string.Format("%02d/%02d", _counter, _limit);
				})
				.AddTo(this);
		}

		private void OnTileMapLoaded()
		{
			if (_tileMap.TileMap.FinishCondition != FinishCondition.MovesLimit &&
			    _tileMap.TileMap.FinishCondition != FinishCondition.TimeAndMoveLimit)
			{
				transform.parent.gameObject.SetActive(false);
				return;
			}

			var condition = _movesFinishConditions.Find(x => x.Id == _tileMap.TileMap.Id);

			if (condition != null)
			{
				_limit = condition.Limit;
			}

			Component.text = string.Format("%02d/%02d", _counter, _limit);
		}
	}
}