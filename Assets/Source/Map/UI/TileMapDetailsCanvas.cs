using System.Collections.Generic;
using System.Linq;
using ModestTree;
using NaughtyAttributes;
using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.General.UI;
using TilesWalk.Map.Bridge;
using TilesWalk.Map.Scaffolding;
using TilesWalk.Map.Tile;
using TMPro;
using TMPro.EditorUtilities;
using UniRx;
using UnityEditor.Build.Pipeline;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TilesWalk.Navigation.UI
{
	public class TileMapDetailsCanvas : CanvasGroupBehaviour, ILevelNameRequire
	{
		[SerializeField] private TextMeshProUGUI _name;
		[SerializeField] private TextMeshProUGUI _target;

		[Header("Navigation")] [SerializeField]
		private Button _nextLevel;

		[SerializeField] private Button _previousLevel;

		[Inject] private List<TileMap> _availableMaps;
		[Inject] private MapLevelBridge _mapLevelBridge;
		[Inject] private LevelTilesHandler _levelTilesHandler;

		public ReactiveProperty<string> LevelName { get; set; } = new ReactiveProperty<string>();
		public TileMap TileMap { get; private set; }

		private void Awake()
		{
			LevelName.Subscribe(level =>
			{
				TileMap = _availableMaps.Find(x => x.Id == level);

				if (TileMap != null)
				{
					LoadMapData();
				}
			}).AddTo(this);

			_nextLevel.onClick.AddListener(OnNextClick);
			_previousLevel.onClick.AddListener(OnPreviousClick);
		}

		private void OnPreviousClick()
		{
			var levelTile = _levelTilesHandler[TileMap];
			var index = _levelTilesHandler.LevelTiles.IndexOf(levelTile);

			if (index > 0)
			{
				levelTile = _levelTilesHandler.LevelTiles[index - 1];
				levelTile.OnMapTileClick(new Unit());
			}
		}

		private void OnNextClick()
		{
			var levelTile = _levelTilesHandler[TileMap];
			var index = _levelTilesHandler.LevelTiles.IndexOf(levelTile);

			if (index != _levelTilesHandler.LevelTiles.Length)
			{
				levelTile = _levelTilesHandler.LevelTiles[index + 1];
				levelTile.OnMapTileClick(new Unit());
			}
		}

		private void LoadMapData()
		{
			_name.text = TileMap.Id;
			_target.text = TileMap.Target.Localize();

			// prepare the bridge
			_mapLevelBridge.SelectedLevel = TileMap;
		}
	}
}