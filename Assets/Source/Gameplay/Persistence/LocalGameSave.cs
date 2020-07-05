using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace TilesWalk.Gameplay.Persistence
{
	/// <summary>
	/// The game save class contains all the data that is meant to
	/// be persistent through all the game instances, this class represents the
	/// local save game data
	/// </summary>
	[Serializable]
	public class LocalGameSave : GameSave
	{
		public LocalGameSave(GameSave save) : base(save)
		{
		}

		public LocalGameSave()
		{
		}

		/// <summary>
		/// This loads the <see cref="GameSave"/> data from a local file
		/// </summary>
		public override void Load()
		{
			try
			{
				var path = Application.persistentDataPath;

				if (Exists("Statistics", path))
				{
					_statistics = Load<GameStatistics>("Statistics", path);
				}

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

				// Begin session on game launch
				_statistics.BeginSession();
				_onGameSaveLoaded?.OnNext(this);
			}
			catch (Exception e)
			{
				Debug.LogWarning(e);
				_onGameSaveLoadFailure?.OnNext(this);
			}
		}

		/// <summary>
		/// This saves the <see cref="GameSave"/> data to a local file
		/// </summary>
		public override void Save()
		{
			try
			{
				var path = Application.persistentDataPath;

				// Update last session timestamp
				_statistics.TimestampSave();
				_statistics.EndSession();

				Save("Statistics", _statistics, path);
				Save("UserMaps", _userMaps, path);
				Save("ImportedMaps", _importedMaps, path);
				Save("GameRecords", _gameRecords, path);
				Save("UserLevelRecords", _userLevelRecords, path);
				Save("ImportedLevelRecords", _importedLevelRecords, path);

				_onGameSaveSaved?.OnNext(this);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				_onGameSaveSaveFailure?.OnNext(this);
			}
		}

		/// <summary>
		/// Save method using <see cref="JsonConvert"/>
		/// </summary>
		/// <typeparam name="T">The data type</typeparam>
		/// <param name="name">Data lookup name</param>
		/// <param name="data">The data meant to be saved</param>
		/// <param name="path">The path where the file will be</param>
		private static void Save<T>(string name, T data, string path)
		{
			FileStream file = File.Create(path + "/" + name);

			if (data != null)
			{
				var content = GameSaveConverter.Serialize(data);
				byte[] info = new UTF8Encoding(true).GetBytes(content);
				file.Write(info, 0, info.Length);
			}

			file.Close();
		}

		/// <summary>
		/// Load method using <see cref="JsonConvert"/>
		/// </summary>
		/// <typeparam name="T">The data type</typeparam>
		/// <param name="name">Data lookup name</param>
		/// <param name="path">The path where the file should be</param>
		private static bool Exists(string name, string path)
		{
			return File.Exists(path + "/" + name);
		}

		/// <summary>
		/// Loads data from a given path
		/// </summary>
		/// <typeparam name="T">The type that the data will be parsed to</typeparam>
		/// <param name="name">Data lookup name</param>
		/// <param name="path">The path where the file should be</param>
		/// <returns></returns>
		private static T Load<T>(string name, string path)
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

					save = GameSaveConverter.Deserialize<T>(content);
				}

				file.Close();
				return save;
			}

			return default(T);
		}
	}
}