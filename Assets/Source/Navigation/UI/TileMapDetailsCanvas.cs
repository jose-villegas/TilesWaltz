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
	public class TileMapDetailsCanvas : CanvasGroupBehaviour
	{
		[SerializeField] private bool _loadFromLevelName;

		[SerializeField, ShowIf("_loadFromLevelName")]
		private string _levelName;

		[SerializeField] private bool _hideAtStart;
		[SerializeField] private TextMeshProUGUI _name;
		[SerializeField] private TextMeshProUGUI _target;
		[SerializeField] private Button _playButton;

		[Inject] private List<TileMap> _availableMaps;
		[Inject] private MapLevelBridge _mapLevelBridge;
		private TileMap _currentTileMap;

		private void Start()
		{
			if (_hideAtStart) Hide();

			_playButton.OnClickAsObservable().Subscribe(OnPlayLevelClick).AddTo(this);

			if (!_loadFromLevelName) return;

			var found = _availableMaps.Find(x => x.Id == _levelName);

			if (found != null) LoadMapData(found);
		}

		private void OnPlayLevelClick(Unit u)
		{
			_mapLevelBridge.SelectedTileMap = _currentTileMap;
		}

		public void LoadMapData(TileMap tileMap)
		{
			_currentTileMap = tileMap;
			_name.text = tileMap.Id;
			_target.text = tileMap.Target.Localize();
		}
	}
}