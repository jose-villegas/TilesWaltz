using System.Collections.Generic;
using System.Linq;
using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Score;
using TilesWalk.General.Patterns;
using TMPro;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class GameLevelsCompletedLabel : ObligatoryComponentBehaviour<TextMeshProUGUI>
	{
		[Inject] private GameScoresHelper _gameScoresHelper;
		[Inject] private List<TileMap> _availableMaps;

		private void Start()
		{
			var maps = _availableMaps.Count(x => x.Target > 0);
			var completed = _availableMaps.Count(x => x.Target > 0 && _gameScoresHelper.IsCompleted(x));

			Component.text = $"{completed.Localize()}/{maps.Localize()}";
		}
	}
}