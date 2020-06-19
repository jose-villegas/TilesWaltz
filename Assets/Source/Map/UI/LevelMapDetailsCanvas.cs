using System.Collections.Generic;
using ModestTree;
using TilesWalk.Building.Level;
using TilesWalk.Extensions;
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
		[SerializeField] private LevelNameRequestHandler _levelRequest;
		[SerializeField] private TextMeshProUGUI _target;
		[SerializeField] private TextMeshProUGUI _stars;
		[SerializeField] private Button _playButton;

		[Header("Navigation")] [SerializeField]
		private Button _nextLevel;

		[SerializeField] private Button _previousLevel;

		[Inject] private List<LevelMap> _availableMaps;
		[Inject] private MapLevelBridge _mapLevelBridge;
		[Inject] private LevelTilesHandler _levelTilesHandler;
		[Inject] private GameScoresHelper _gameScoresHelper;

		public LevelNameRequestHandler LevelRequest => _levelRequest;


		private void Awake()
		{
			_levelRequest.OnTileMapFoundAsObservable().Subscribe(level => { LoadMapData(); }).AddTo(this);
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

		private void LoadMapData()
		{
			_target.text = _levelRequest.Map.Target.Localize();
			_stars.text = $"{_gameScoresHelper.GameStars}/{_levelRequest.Map.StarsRequired}";
			_playButton.interactable = _gameScoresHelper.GameStars >= _levelRequest.Map.StarsRequired;

			// prepare the bridge
			_mapLevelBridge.SelectedLevel = _levelRequest.Map;

			// set navigation buttons
			var levelTile = _levelTilesHandler[_levelRequest.Map];
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