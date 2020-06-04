using System.Collections.Generic;
using NaughtyAttributes;
using TilesWalk.Building.Map;
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
	[RequireComponent(typeof(CanvasGroup))]
	public class TileMapDetails : ObligatoryComponentBehaviour<CanvasGroup>
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

		public bool IsVisible => Component.alpha > 0;

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
			_target.text = tileMap.Target.ToString();
		}

		public void Hide()
		{
			Component.alpha = 0;
			Component.interactable = false;
		}

		public void Show()
		{
			Component.alpha = 1;
			Component.interactable = true;
		}
	}
}