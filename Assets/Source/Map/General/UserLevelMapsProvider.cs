using System.Collections.Generic;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Level;
using TilesWalk.Gameplay.Persistence;
using UnityEngine;
using Zenject;

namespace TilesWalk.Map.General
{
	public class UserLevelMapsProvider : MonoBehaviour, IMapProvider
	{
		[Inject] private GameSave _gameSave;
		[Inject] private CustomLevelsConfiguration _configuration;

		public GameMapCollection Collection => _gameSave.UserMaps;
		public RecordsKeeper Records => _gameSave.UserLevelRecords;
		public int MaximumLevels => _configuration.MaximumUserMaps;
	}
}