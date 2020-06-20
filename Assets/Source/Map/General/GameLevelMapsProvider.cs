using System.Collections.Generic;
using TilesWalk.Building.Level;
using UnityEngine;
using Zenject;

namespace TilesWalk.Map.General
{
	public class GameLevelMapsProvider : MonoBehaviour, IMapProvider
	{
		[Inject] private List<LevelMap> _availableMaps;

		public List<LevelMap> AvailableMaps => _availableMaps;
	}
}