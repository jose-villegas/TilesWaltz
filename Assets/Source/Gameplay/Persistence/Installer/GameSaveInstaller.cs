using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Persistence.Installer
{
	public class GameSaveInstaller : MonoInstaller
	{
		[SerializeField] private GameSave _gameSave;

		private LocalGameSave _localSave = new LocalGameSave();
		private CloudSaveGame _cloudSave = new CloudSaveGame();
        private bool _bound;

        public override void InstallBindings()
		{
			_localSave.OnGameSaveLoadedAsObservable().Subscribe(OnLocalSaveLoaded).AddTo(this);
			_localSave.OnGameSaveLoadFailureAsObservable().Subscribe(OnLocalSaveLoadFailure).AddTo(this);

			_localSave.Load();
		}

		private void OnLocalSaveLoadFailure(GameSave obj)
		{
			// since local load failed, create an instance
			_localSave = new LocalGameSave();
		}

		private void OnLocalSaveLoaded(GameSave save)
		{
			_localSave = save as LocalGameSave;

			// after local save is loaded try to load the cloud save
			_cloudSave.OnGameSaveLoadedAsObservable().Subscribe(OnCloudSaveLoaded).AddTo(this);
			_cloudSave.OnGameSaveLoadFailureAsObservable().Subscribe(OnCloudSaveLoadFailure).AddTo(this);

			_cloudSave.Load();
		}

		private void OnCloudSaveLoaded(GameSave save)
		{
			_cloudSave = save as CloudSaveGame;

			// compare timestamps to check which one has the latest
			var cloudTime = _cloudSave.Statistics.LastSaveTimestamp;
			var localTime = _localSave.Statistics.LastSaveTimestamp;

			// cloud is newer, use cloud save
			if (cloudTime > localTime)
			{
				_gameSave = _cloudSave;
			}
			else
			{
				_gameSave = _localSave;
			}

            if (!_bound)
            {
                _bound = true;
                Container.Bind<GameSave>().FromInstance(_gameSave).AsSingle();
			}
		}

		private void OnCloudSaveLoadFailure(GameSave save)
		{
			// since the cloud save failed, prefer the local save as savepoint
			_gameSave = _localSave;

            if (!_bound)
            {
                _bound = true;
                Container.Bind<GameSave>().FromInstance(_gameSave).AsSingle();
            }
		}

		private void OnApplicationPause(bool _)
		{
			if (_gameSave == null) return;

			new LocalGameSave(_gameSave).Save();
			new CloudSaveGame(_gameSave).Save();
		}

		private void OnApplicationQuit()
		{
			if (_gameSave == null) return;

			new LocalGameSave(_gameSave).Save();
			new CloudSaveGame(_gameSave).Save();
		}
	}
}