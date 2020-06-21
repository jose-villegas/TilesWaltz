using System.Collections.Generic;
using ModestTree;
using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Condition;
using TilesWalk.Gameplay.Score;
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
		private Button _nextLevel;

		[SerializeField] private Button _previousLevel;

		public LevelNameRequestHandler LevelRequest => _levelRequest;

		private void Awake()
		{
			_levelRequest.OnTileMapFoundAsObservable().Subscribe(UpdateCanvas).AddTo(this);
			_nextLevel.onClick.AddListener(OnNextClick);
			_previousLevel.onClick.AddListener(OnPreviousClick);
		}

		private void OnPreviousClick()
		{
			var levelTile = _levelTilesHandler[_levelRequest.Map];
			var index = _levelTilesHandler.LevelTiles.IndexOf(levelTile);

			if (index > 0)
			{
				levelTile = _levelTilesHandler.LevelTiles[index - 1];
				levelTile.OnMapTileClick();
			}
		}

		private void OnNextClick()
		{
			var levelTile = _levelTilesHandler[_levelRequest.Map];
			var index = _levelTilesHandler.LevelTiles.IndexOf(levelTile);

			if (index != _levelTilesHandler.LevelTiles.Length)
			{
				levelTile = _levelTilesHandler.LevelTiles[index + 1];
				levelTile.OnMapTileClick();
			}
		}

		private void UpdateCanvas(LevelMap map)
		{
			_playButton.interactable = _gameScoresHelper.GameStars >= map.StarsRequired;

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
			var index = _levelTilesHandler.LevelTiles.IndexOf(levelTile);

			_nextLevel.interactable = true;
			_previousLevel.interactable = true;

			if (index == 0)
			{
				_nextLevel.interactable = true;
				_previousLevel.interactable = false;
			}

			if (index == _levelTilesHandler.LevelTiles.Length - 1)
			{
				_nextLevel.interactable = false;
				_previousLevel.interactable = true;
			}
		}
	}
}