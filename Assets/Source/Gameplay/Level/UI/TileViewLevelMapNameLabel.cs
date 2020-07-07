using TilesWalk.Building.Level;
using TilesWalk.General.Patterns;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Level.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class TileViewLevelMapNameLabel : ObligatoryComponentBehaviour<TextMeshProUGUI>
	{
		[Inject] private TileViewLevelMap _tileLevelMap;

		private void Start()
		{
			if (_tileLevelMap.Map != null && !string.IsNullOrEmpty(_tileLevelMap.Map.Id))
			{
				Component.text = _tileLevelMap.Map.Id;
			}

			_tileLevelMap.OnLevelMapLoadedAsObservable().Subscribe(OnLevelMapLoaded).AddTo(this);
		}

		private void OnLevelMapLoaded(LevelMap levelMap)
		{
			Component.text = levelMap.Id;
		}
	}
}