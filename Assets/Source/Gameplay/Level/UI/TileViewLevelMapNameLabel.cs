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
			if (_tileViewLevelMap.LevelMap != null && !string.IsNullOrEmpty(_tileViewLevelMap.LevelMap.Id))
			{
				Component.text = _tileViewLevelMap.LevelMap.Id;
			}

			_tileViewLevelMap.OnLevelMapLoadedAsObservable().Subscribe(OnLevelMapLoaded).AddTo(this);
		}

		private void OnLevelMapLoaded(LevelMap levelMap)
		{
			Component.text = levelMap.Id;
		}
	}
}