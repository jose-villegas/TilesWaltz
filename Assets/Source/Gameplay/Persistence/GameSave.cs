using System.Collections;
using BayatGames.SaveGameFree;
using TilesWalk.Gameplay.Score;
using UnityEditor;

namespace TilesWalk.Gameplay.Persistence
{
	public class GameSave
	{
		private RecordsKeeper _records;
		private RecordsKeeper _customRecords;
		private CustomMaps _userMaps;
		private CustomMaps _importedMaps;

		public RecordsKeeper Records => _records;
		public RecordsKeeper CustomRecords => _customRecords;
		public CustomMaps UserMaps => _userMaps;
		public CustomMaps ImportedMaps => _importedMaps;

		public GameSave()
		{
			_customRecords = new RecordsKeeper();
			_records = new RecordsKeeper();
			_userMaps = new CustomMaps();
			_importedMaps = new CustomMaps();
		}

		public void LoadFromLocal()
		{
			if (SaveGame.Exists("UserMaps"))
			{
				_userMaps = SaveGame.Load<CustomMaps>("UserMaps");
			}

			if (SaveGame.Exists("ImportedMaps"))
			{
				_importedMaps = SaveGame.Load<CustomMaps>("ImportedMaps");
			}

			if (SaveGame.Exists("Records"))
			{
				_records = SaveGame.Load<RecordsKeeper>("Records");
			}

			if (SaveGame.Exists("CustomRecords"))
			{
				_records = SaveGame.Load<RecordsKeeper>("CustomRecords");
			}
		}

		public void SaveToLocal()
		{
			SaveGame.Save("UserMaps", _userMaps);
			SaveGame.Save("ImportedMaps", _importedMaps);
			SaveGame.Save("Records", _records);
			SaveGame.Save("CustomRecords", _customRecords);
		}
	}
}