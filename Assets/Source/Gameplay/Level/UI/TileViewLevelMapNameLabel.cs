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
		[Inject] private TileViewLevelMap _tileViewLevelMap;

		private void Start()
		{
			if (_tileViewLevelMap.Map != null && !string.IsNullOrEmpty(_tileViewLevelMap.Map.Id))
			{
				Component.text = _tileViewLevelMap.Map.Id;
			}

			_tileViewLevelMap.OnLevelMapLoadedAsObservable().Subscribe(OnLevelMapLoaded).AddTo(this);
		}

		private void OnLevelMapLoaded(LevelMap levelMap)
		{
			Component.text = levelMap.Id;
		}
	}
}