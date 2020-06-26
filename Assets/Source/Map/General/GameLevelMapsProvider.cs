using System.Collections.Generic;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Persistence;
using UnityEngine;
using Zenject;

namespace TilesWalk.Map.General
{
	public class GameLevelMapsProvider : MonoBehaviour, IMapProvider
	{
		[Inject(Id = "GameMaps")] private GameMapCollection _gameMaps;
		[Inject] private GameSave _gameSave;

		public GameMapCollection Collection => _gameMaps;
		public RecordsKeeper Records => _gameSave.GameRecords;
		public int MaximumLevels => int.MaxValue;
	}
}