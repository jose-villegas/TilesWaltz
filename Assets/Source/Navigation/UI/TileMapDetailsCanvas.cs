using System.Collections.Generic;
using NaughtyAttributes;
using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.General.UI;
using TilesWalk.Navigation.Map;
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
		[SerializeField] private Button _playButton;
		[Header("Navigation")]
		[SerializeField] private Button _nextLevel;
		[SerializeField] private Button _previousLevel;

		[Inject] private List<TileMap> _availableMaps;
		[Inject] private MapLevelBridge _mapLevelBridge;

		public ReactiveProperty<string> LevelName { get; set; } =  new ReactiveProperty<string>();
		public TileMap TileMap { get; private set; }


		private void Start()
		{
			LevelName.Subscribe(level =>
			{
				TileMap = _availableMaps.Find(x => x.Id == level);

				if (TileMap != null)
				{
					LoadMapData();
				}
			}).AddTo(this);
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