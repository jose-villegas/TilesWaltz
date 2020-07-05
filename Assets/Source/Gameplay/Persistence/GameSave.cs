using System;
using System.Runtime.Serialization.Formatters.Binary;
using UniRx;
using UnityEngine;

namespace TilesWalk.Gameplay.Persistence
{
	/// <summary>
	/// The game save class contains all the data that is meant to
	/// be persistent through all the game instances
	/// </summary>
	[Serializable]
	public abstract class GameSave
	{
		[SerializeField] protected GameStatistics _statistics;

		[SerializeField] protected RecordsKeeper _gameRecords;
		[SerializeField] protected RecordsKeeper _userLevelRecords;
		[SerializeField] protected RecordsKeeper _importedLevelRecords;

		[SerializeField] protected GameMapCollection _userMaps;
		[SerializeField] protected GameMapCollection _importedMaps;

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

		protected Subject<GameSave> _onGameSaveLoadFailure;
		protected Subject<GameSave> _onGameSaveLoaded;
		protected Subject<GameSave> _onGameSaveSaveFailure;
		protected Subject<GameSave> _onGameSaveSaved;

		public GameSave()
		{
			_statistics = new GameStatistics();

			_gameRecords = new RecordsKeeper();
			_userLevelRecords = new RecordsKeeper();
			_importedLevelRecords = new RecordsKeeper();

			_userMaps = new GameMapCollection();
			_importedMaps = new GameMapCollection();
		}

		~GameSave()
		{
			_onGameSaveSaved?.OnCompleted();
			_onGameSaveLoaded?.OnCompleted();
			_onGameSaveSaveFailure?.OnCompleted();
			_onGameSaveLoadFailure?.OnCompleted();
		}

		public IObservable<GameSave> OnGameSaveLoadedAsObservable()
		{
			return _onGameSaveLoaded = _onGameSaveLoaded == null ? new Subject<GameSave>() : _onGameSaveLoaded;
		}

		public IObservable<GameSave> OnGameSaveSavedAsObservable()
		{
			return _onGameSaveSaved = _onGameSaveSaved == null ? new Subject<GameSave>() : _onGameSaveSaved;
		}

		public IObservable<GameSave> OnGameSaveSaveFailureAsObservable()
		{
			return _onGameSaveSaveFailure =
				_onGameSaveSaveFailure == null ? new Subject<GameSave>() : _onGameSaveSaveFailure;
		}

		public IObservable<GameSave> OnGameSaveLoadFailureAsObservable()
		{
			return _onGameSaveLoadFailure =
				_onGameSaveLoadFailure == null ? new Subject<GameSave>() : _onGameSaveLoadFailure;
		}

		/// <summary>
		/// This saves the <see cref="GameSave"/> data.
		/// </summary>
		public abstract void Save();

		/// <summary>
		/// This loads the <see cref="GameSave"/> data.
		/// </summary>
		public abstract void Load();

		public GameSave(GameSave save)
		{
			_statistics = save._statistics;
			_gameRecords = save._gameRecords;
			_userLevelRecords = save._userLevelRecords;
			_importedLevelRecords = save._importedLevelRecords;
			_userMaps = save._userMaps;
			_importedMaps = save._importedMaps;
		}
	}
}