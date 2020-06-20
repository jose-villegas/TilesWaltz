using System.Collections.Generic;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Persistence;
using UnityEngine;
using Zenject;

namespace TilesWalk.Map.General
{
	public class UserLevelMapsProvider : MonoBehaviour, IMapProvider
	{
		[Inject] private GameSave _gameSave;

		public List<LevelMap> AvailableMaps => _gameSave.UserMaps.AvailableMaps;
	}
}