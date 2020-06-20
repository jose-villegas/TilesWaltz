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
	public class GameStarsLabel : ObligatoryComponentBehaviour<TextMeshProUGUI>
	{
		[Inject] private GameScoresHelper _gameScoresHelper;

		[SerializeField] private MapProviderSolver _solver;

		private void Start()
		{
			if (_solver == null) _solver = new MapProviderSolver(gameObject);

			_solver.InstanceProvider();

			var maps = _solver.Provider.AvailableMaps.Count;

			Component.text = $"{_gameScoresHelper.GameStars.Localize()}/{(maps * 3).Localize()}";
		}
	}
}