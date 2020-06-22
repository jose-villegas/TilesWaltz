using TilesWalk.Gameplay.Persistence;
using UnityEngine;
using Zenject;

namespace TilesWalk.Map.General
{
	public class ImportedLevelMapsProvider : MonoBehaviour, IMapProvider
	{
		[Inject] private GameSave _gameSave;

		public GameMapCollection Collection => _gameSave.ImportedMaps;
		public RecordsKeeper Records => _gameSave.ImportedLevelRecords;
	}
}