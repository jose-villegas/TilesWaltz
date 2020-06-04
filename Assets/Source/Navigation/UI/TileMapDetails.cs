using System.Collections.Generic;
using NaughtyAttributes;
using TilesWalk.Building.Map;
using TMPro;
using UnityEditor.Build.Pipeline;
using UnityEngine;
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

		[Inject] private List<TileMap> _availableMaps;

		public bool IsVisible => Component.alpha > 0;

		private void Start()
		{
			if (_hideAtStart) Hide();

			if (!_loadFromLevelName) return;

			var found = _availableMaps.Find(x => x.Id == _levelName);

			if (found != null) LoadMapData(found);
		}

		public void LoadMapData(TileMap tileMap)
		{
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