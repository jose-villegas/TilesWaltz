using System;
using System.Collections;
using BayatGames.SaveGameFree;
using BayatGames.SaveGameFree.Serializers;
using Newtonsoft.Json;
using TilesWalk.Gameplay.Score;
using UnityEditor;

namespace TilesWalk.Gameplay.Persistence
{
	[Serializable]
	public class GameSave
	{
		private RecordsKeeper _gameRecords;
		private RecordsKeeper _userLevelRecords;
		private RecordsKeeper _importedLevelRecords;
		private GameMapCollection _userMaps;
		private GameMapCollection _importedMaps;

		public RecordsKeeper GameRecords => _gameRecords;
		public RecordsKeeper UserLevelRecords => _userLevelRecords;
		public RecordsKeeper ImportedLevelRecords => _importedLevelRecords;
		public GameMapCollection UserMaps => _userMaps;
		public GameMapCollection ImportedMaps => _importedMaps;


		public GameSave()
		{
			SaveGame.Serializer = new JsonSerializer();

			_userLevelRecords = new RecordsKeeper();
			_gameRecords = new RecordsKeeper();
			_userMaps = new GameMapCollection();
			_importedMaps = new GameMapCollection();
		}

		public void LoadFromLocal()
		{
			if (SaveGame.Exists("UserMaps"))
			{
				_userMaps = SaveGame.Load<GameMapCollection>("UserMaps");
			}

			if (SaveGame.Exists("ImportedMaps"))
			{
				_importedMaps = SaveGame.Load<GameMapCollection>("ImportedMaps");
			}

			if (SaveGame.Exists("GameRecords"))
			{
				_gameRecords = SaveGame.Load<RecordsKeeper>("GameRecords");
			}

			if (SaveGame.Exists("UserLevelRecords"))
			{
				_userLevelRecords = SaveGame.Load<RecordsKeeper>("UserLevelRecords");
			}

			if (SaveGame.Exists("ImportedLevelRecords"))
			{
				_importedLevelRecords = SaveGame.Load<RecordsKeeper>("ImportedLevelRecords");
			}
		}

		public void SaveToLocal()
		{
			SaveGame.Save("UserMaps", _userMaps);
			SaveGame.Save("ImportedMaps", _importedMaps);
			SaveGame.Save("GameRecords", _gameRecords);
			SaveGame.Save("UserLevelRecords", _userLevelRecords);
			SaveGame.Save("ImportedLevelRecords", _importedLevelRecords);
		}
	}
}