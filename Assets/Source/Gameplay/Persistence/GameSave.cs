using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Newtonsoft.Json;
using UniRx;
using UnityEngine;

namespace TilesWalk.Gameplay.Persistence
{
	/// <summary>
	/// The game save class contains all the data that is meant to
	/// be persistent through all the game instances
	/// </summary>
	[Serializable]
	public class GameSave
	{
		[SerializeField] private GameStatistics _statistics;
		[SerializeField] private RecordsKeeper _gameRecords;
		[SerializeField] private RecordsKeeper _userLevelRecords;
		[SerializeField] private RecordsKeeper _importedLevelRecords;
		[SerializeField] private GameMapCollection _userMaps;
		[SerializeField] private GameMapCollection _importedMaps;

		/// <summary>
		/// Contains user related statistics
		/// </summary>
		public GameStatistics Statistics => _statistics;
		/// <summary>
		/// Records for the game levels, meaning levels that come bundled with the game
		/// and were not made by the user or others
		/// </summary>
		public RecordsKeeper GameRecords => _gameRecords;
		/// <summary>
		/// Records for levels made by the user
		/// </summary>
		public RecordsKeeper UserLevelRecords => _userLevelRecords;
		/// <summary>
		/// Records for levels made by other users
		/// </summary>
		public RecordsKeeper ImportedLevelRecords => _importedLevelRecords;
		/// <summary>
		/// Levels made by the user
		/// </summary>
		public GameMapCollection UserMaps => _userMaps;
		/// <summary>
		/// Imported maps made by other users
		/// </summary>
		public GameMapCollection ImportedMaps => _importedMaps;

		private Subject<GameSave> _onGameSaveLoaded;
		private Subject<GameSave> _onGameSaveSaved;

		public GameSave()
		{
			_userLevelRecords = new RecordsKeeper();
			_gameRecords = new RecordsKeeper();
			_userMaps = new GameMapCollection();
			_importedMaps = new GameMapCollection();
		}

		/// <summary>
		/// This loads the <see cref="GameSave"/> data from a local file
		/// </summary>
		public void LoadFromLocal()
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

		/// <summary>
		/// This saves the <see cref="GameSave"/> data to a local file
		/// </summary>
		public void SaveToLocal()
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

		public IObservable<GameSave> OnGameSaveLoadedAsObservable()
		{
			return _onGameSaveLoaded = _onGameSaveLoaded == null ? new Subject<GameSave>() : _onGameSaveLoaded;
		}

		public IObservable<GameSave> OnGameSaveSavedAsObservable()
		{
			return _onGameSaveSaved = _onGameSaveSaved == null ? new Subject<GameSave>() : _onGameSaveSaved;
		}

		~GameSave()
		{
			_onGameSaveSaved?.OnCompleted();
			_onGameSaveLoaded?.OnCompleted();
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
				var content = JsonConvert.SerializeObject(data);
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

					save = JsonConvert.DeserializeObject<T>(content);
				}

				file.Close();
				return save;
			}

			return default(T);
		}
	}
}