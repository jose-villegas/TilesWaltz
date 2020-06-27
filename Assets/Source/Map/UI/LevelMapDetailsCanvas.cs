using System;
using System.Collections.Generic;
using ModestTree;
using TilesWalk.Building.Level;
using TilesWalk.Building.LevelEditor.UI;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Condition;
using TilesWalk.Gameplay.Score;
using TilesWalk.General;
using TilesWalk.General.UI;
using TilesWalk.Map.Bridge;
using TilesWalk.Map.Scaffolding;
using TilesWalk.Map.Tile;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TilesWalk.Navigation.UI
{
	public class LevelMapDetailsCanvas : CanvasGroupBehaviour
	{
		[Inject] private LevelBridge _levelBridge;
		[Inject] private LevelTilesHandler _levelTilesHandler;
		[Inject] private GameScoresHelper _gameScoresHelper;

		[SerializeField] private LevelNameRequestHandler _levelRequest;
		[SerializeField] private Button _playButton;
		[SerializeField] private CanvasGroupBehaviour _timeConditionContainer;
		[SerializeField] private CanvasGroupBehaviour _moveConditionContainer;

		[Header("Navigation")] [SerializeField]
		private List<DirectionButton> _directionButtons;

		public LevelNameRequestHandler LevelRequest => _levelRequest;

		private void Awake()
		{
			_levelRequest.OnTileMapFoundAsObservable().Subscribe(UpdateCanvas).AddTo(this);

			for (int i = 0; i < _directionButtons.Count; i++)
			{
				var directionButton = _directionButtons[i];

				directionButton.Button.onClick.AddListener(() =>
				{
					switch (directionButton.Direction)
					{
						case CardinalDirection.North:
							EnterDirection = Vector2.down * 3;
							ExitDirection = -EnterDirection;
							break;
						case CardinalDirection.South:
							EnterDirection = Vector2.up * 3;
							ExitDirection = -EnterDirection;
							break;
						case CardinalDirection.East:
							EnterDirection = Vector2.left * 3;
							ExitDirection = -EnterDirection;
							break;
						case CardinalDirection.West:
							EnterDirection = Vector2.right * 3;
							ExitDirection = -EnterDirection;
							break;
					}


					var levelTile = _levelTilesHandler[_levelRequest.Map];
					var neighbor = levelTile[directionButton.Direction];

					OnHideAsObservable().Take(1).Subscribe(u =>
					{
						neighbor.OnMapTileClick();

					}).AddTo(this);
		
					Hide();
				});
			}
		}

		private void UpdateCanvas(LevelMap map)
		{
			//_playButton.interactable = _gameScoresHelper.GameStars >= map.StarsRequired;

			// prepare the bridge
			_levelBridge.Payload = new LevelBridgePayload(_levelRequest.Map, _levelRequest.Condition);

			//// set condition
			if (map.FinishCondition == FinishCondition.MovesLimit)
			{
				_moveConditionContainer.Show();
				_timeConditionContainer.Hide();
			}
			else if (map.FinishCondition == FinishCondition.TimeLimit)
			{
				_moveConditionContainer.Hide();
				_timeConditionContainer.Show();
			}

			// set navigation buttons
			var levelTile = _levelTilesHandler[map];

			for (int i = 0; i < _directionButtons.Count; i++)
			{
				var directionButton = _directionButtons[i];
				directionButton.Button.image.enabled = levelTile.HasNeighbor(directionButton.Direction);
				directionButton.Button.interactable = levelTile.HasNeighbor(directionButton.Direction);
			}
		}
	}
}