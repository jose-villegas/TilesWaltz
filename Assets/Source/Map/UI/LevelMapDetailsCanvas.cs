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
	public class LevelMapDetailsCanvas : CanvasGroupBehaviour, ILevelNameRequire
	{
		[SerializeField] private TextMeshProUGUI _name;
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

		public ReactiveProperty<string> LevelName { get; set; } = new ReactiveProperty<string>();
		public LevelMap LevelMap { get; private set; }

		private void Awake()
		{
			LevelName.Subscribe(level =>
			{
				LevelMap = _availableMaps.Find(x => x.Id == level);

				if (LevelMap != null)
				{
					LoadMapData();
				}
			}).AddTo(this);

			_nextLevel.onClick.AddListener(OnNextClick);
			_previousLevel.onClick.AddListener(OnPreviousClick);
		}

		private void OnPreviousClick()
		{
			var levelTile = _levelTilesHandler[LevelMap];
			var index = _levelTilesHandler.LevelTiles.IndexOf(levelTile);

			if (index > 0)
			{
				levelTile = _levelTilesHandler.LevelTiles[index - 1];
				levelTile.OnMapTileClick();
			}
		}

		private void OnNextClick()
		{
			var levelTile = _levelTilesHandler[LevelMap];
			var index = _levelTilesHandler.LevelTiles.IndexOf(levelTile);

			if (index != _levelTilesHandler.LevelTiles.Length)
			{
				levelTile = _levelTilesHandler.LevelTiles[index + 1];
				levelTile.OnMapTileClick();
			}
		}

		private void LoadMapData()
		{
			_name.text = LevelMap.Id;
			_target.text = LevelMap.Target.Localize();
			_stars.text = $"{_gameScoresHelper.GameStars}/{LevelMap.StarsRequired}";
			_playButton.interactable = _gameScoresHelper.GameStars >= LevelMap.StarsRequired;

			// prepare the bridge
			_mapLevelBridge.SelectedLevel = LevelMap;

			// set navigation buttons
			var levelTile = _levelTilesHandler[LevelMap];
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