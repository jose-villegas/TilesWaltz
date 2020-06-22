using System.Collections.Generic;
using System.Linq;
using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Score;
using TilesWalk.General.Patterns;
using TilesWalk.Map.General;
using TMPro;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.UI
{
	[RequireComponent(typeof(TextMeshProUGUI), typeof(IMapProvider))]
	public class GameLevelsCompletedLabel : ObligatoryComponentBehaviour<TextMeshProUGUI>
	{
		[Inject] private GameScoresHelper _gameScoresHelper;
		[Inject] private MapProviderSolver _solver;

		private void Start()
		{
			_solver.InstanceProvider(gameObject);

			var maps = _solver.Provider.Collection.AvailableMaps.Count;
			var completed = _solver.Provider.Collection.AvailableMaps.Count(x => _gameScoresHelper.IsCompleted(x));

			Component.text = $"{completed.Localize()}/{maps.Localize()}";
		}
	}
}