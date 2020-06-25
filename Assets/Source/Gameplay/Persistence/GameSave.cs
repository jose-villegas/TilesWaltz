using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

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
			_userLevelRecords = new RecordsKeeper();
			_gameRecords = new RecordsKeeper();
			_userMaps = new GameMapCollection();
			_importedMaps = new GameMapCollection();
		}

		public void LoadFromLocal()
		{
			var path = Application.persistentDataPath;

			if (Exists("UserMaps", path))
			{
				_userMaps = Load<GameMapCollection>("UserMaps", path);
			}

			if (Exists("ImportedMaps", path))
			{
				_importedMaps = Load<GameMapCollection>("ImportedMaps", path);
			}

			if (Exists("GameRecords", path))
			{
				_gameRecords = Load<RecordsKeeper>("GameRecords", path);
			}

			if (Exists("UserLevelRecords", path))
			{
				_userLevelRecords = Load<RecordsKeeper>("UserLevelRecords", path);
			}

			if (Exists("ImportedLevelRecords", path))
			{
				_importedLevelRecords = Load<RecordsKeeper>("ImportedLevelRecords", path);
			}
		}

		public void SaveToLocal()
		{
			var path = Application.persistentDataPath;
			Save("UserMaps", _userMaps, path);
			Save("ImportedMaps", _importedMaps, path);
			Save("GameRecords", _gameRecords, path);
			Save("UserLevelRecords", _userLevelRecords, path);
			Save("ImportedLevelRecords", _importedLevelRecords, path);
		}

		public static void Save<T>(string name, T data, string path)
		{
			FileStream file = File.Create(path + "/" + name);

			if (data != null)
			{
				var content = JsonConvert.SerializeObject(data);
				byte[] info = new UTF8Encoding(true).GetBytes(content);
				file.Write(info, 0, info.Length);
			}

			file.Close();
		}

		public static bool Exists(string name, string path)
		{
			return File.Exists(path + "/" + name);
		}

		public static T Load<T>(string name, string path)
		{
			if (File.Exists(path + "/" + name))
			{
				FileStream file = File.Open(path + "/" + name, FileMode.Open);
				T save = default(T);

				if (file.Length >= 0)
				{
					string content;

					using (StreamReader reader = new StreamReader(file))
					{
						content = reader.ReadToEnd();
					}

					save = JsonConvert.DeserializeObject<T>(content);
				}

				file.Close();
				return save;
			}

			return default(T);
		}
	}
}