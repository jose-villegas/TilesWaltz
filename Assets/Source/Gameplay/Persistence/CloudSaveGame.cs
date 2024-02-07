using System;
using System.Collections.Generic;
using System.Linq;
using TilesWalk.GPGS;
using UniRx;
using UnityEngine;

namespace TilesWalk.Gameplay.Persistence
{
	[Serializable]
	public class CloudSaveGame : GameSave
	{
		private Subject<string> _saveAwaiter;
		private List<string> _savedFiles;
		private IDisposable _saveAwaiterDisposable;

		private string[] DataFiles =
		{
			"Statistics", "UserMaps", "ImportedMaps",
			"GameRecords", "UserLevelRecords", "ImportedLevelRecords"
		};

		public CloudSaveGame(GameSave save) : base(save)
		{
		}

		public CloudSaveGame()
		{
		}

		public override void Save()
		{
			// Update last session timestamp
			_statistics.TimestampSave();
			_statistics.EndSession();

			_saveAwaiterDisposable?.Dispose();
			_savedFiles = new List<string>();
			_saveAwaiter = new Subject<string>();

			var data = GameSaveConverter.Serialize(_statistics);
			GPGSCloudSave.SaveToCloud(DataFiles[0], data,
				() => _saveAwaiter.OnNext(DataFiles[0]),
				() => _onGameSaveSaveFailure?.OnNext(this));

			data = GameSaveConverter.Serialize(_userMaps);
			GPGSCloudSave.SaveToCloud(DataFiles[1], data,
				() => _saveAwaiter.OnNext(DataFiles[1]),
				() => _onGameSaveSaveFailure?.OnNext(this));

			data = GameSaveConverter.Serialize(_importedMaps);
			GPGSCloudSave.SaveToCloud(DataFiles[2], data,
				() => _saveAwaiter.OnNext(DataFiles[2]),
				() => _onGameSaveSaveFailure?.OnNext(this));

			data = GameSaveConverter.Serialize(_gameRecords);
			GPGSCloudSave.SaveToCloud(DataFiles[3], data,
				() => _saveAwaiter.OnNext(DataFiles[3]),
				() => _onGameSaveSaveFailure?.OnNext(this));

			data = GameSaveConverter.Serialize(_userLevelRecords);
			GPGSCloudSave.SaveToCloud(DataFiles[4], data,
				() => _saveAwaiter.OnNext(DataFiles[4]),
				() => _onGameSaveSaveFailure?.OnNext(this));

			data = GameSaveConverter.Serialize(_importedLevelRecords);
			GPGSCloudSave.SaveToCloud(DataFiles[5], data, () => _saveAwaiter.OnNext(DataFiles[5]),
				() => _onGameSaveSaveFailure?.OnNext(this));

			_saveAwaiterDisposable = _saveAwaiter.Subscribe(name =>
			{
				if (_savedFiles == null) _savedFiles = new List<string>();

				_savedFiles.Add(name);

				if (_savedFiles.Count == 6 && _savedFiles.All(item => DataFiles.Contains(item)))
				{
					_onGameSaveSaved?.OnNext(this);
				}
			});
		}

		public override void Load()
		{
			_saveAwaiterDisposable?.Dispose();
			_savedFiles = new List<string>();
			_saveAwaiter = new Subject<string>();

			GPGSCloudSave.LoadFromCloud(DataFiles[0],
				(result) =>
				{
					try
					{
						_statistics = GameSaveConverter.Deserialize<GameStatistics>(result);
						_saveAwaiter.OnNext(DataFiles[0]);
					}
					catch (Exception e)
					{
						_onGameSaveLoadFailure?.OnNext(this);
						Debug.LogWarning(e);
					}
				},
				() => _onGameSaveLoadFailure?.OnNext(this));

			GPGSCloudSave.LoadFromCloud(DataFiles[1],
				(result) =>
				{
					try
					{
						_userMaps = GameSaveConverter.Deserialize<GameMapCollection>(result);
						_saveAwaiter.OnNext(DataFiles[1]);
					}
					catch (Exception e)
					{
						_onGameSaveLoadFailure?.OnNext(this);
						Debug.LogWarning(e);
					}
				},
				() => _onGameSaveLoadFailure?.OnNext(this));

			GPGSCloudSave.LoadFromCloud(DataFiles[2],
				(result) =>
				{
					try
					{
						_importedMaps = GameSaveConverter.Deserialize<GameMapCollection>(result);
						_saveAwaiter.OnNext(DataFiles[2]);
					}
					catch (Exception e)
					{
						_onGameSaveLoadFailure?.OnNext(this);
						Debug.LogWarning(e);
					}
				},
				() => _onGameSaveLoadFailure?.OnNext(this));

			GPGSCloudSave.LoadFromCloud(DataFiles[3],
				(result) =>
				{
					try
					{
						_gameRecords = GameSaveConverter.Deserialize<RecordsKeeper>(result);
						_saveAwaiter.OnNext(DataFiles[3]);
					}
					catch (Exception e)
					{
						_onGameSaveLoadFailure?.OnNext(this);
						Debug.LogWarning(e);
					}
				},
				() => _onGameSaveLoadFailure?.OnNext(this));

			GPGSCloudSave.LoadFromCloud(DataFiles[4],
				(result) =>
				{
					try
					{
						_userLevelRecords = GameSaveConverter.Deserialize<RecordsKeeper>(result);
						_saveAwaiter.OnNext(DataFiles[4]);
					}
					catch (Exception e)
					{
						_onGameSaveLoadFailure?.OnNext(this);
						Debug.LogWarning(e);
					}
				},
				() => _onGameSaveLoadFailure?.OnNext(this));

			GPGSCloudSave.LoadFromCloud(DataFiles[5],
				(result) =>
				{
					try
					{
						_importedLevelRecords = GameSaveConverter.Deserialize<RecordsKeeper>(result);
						_saveAwaiter.OnNext(DataFiles[5]);
					}
					catch (Exception e)
					{
						_onGameSaveLoadFailure?.OnNext(this);
						Debug.LogWarning(e);
					}
				},
				() => _onGameSaveLoadFailure?.OnNext(this));

			_saveAwaiterDisposable = _saveAwaiter.Subscribe(name =>
			{
				if (_savedFiles == null) _savedFiles = new List<string>();

				_savedFiles.Add(name);

				if (_savedFiles.Count == 6 && _savedFiles.All(item => DataFiles.Contains(item)))
				{
					// Begin session on game launch
					_statistics.BeginSession();
					_onGameSaveLoaded?.OnNext(this);
				}
			});
		}
	}
}