using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Persistence.Installer
{
	public class GameSaveInstaller : MonoInstaller
	{
		[SerializeField] private GameSave _gameSave = new GameSave();

		public override void InstallBindings()
		{
			_gameSave.LoadFromLocal();
			Container.Bind<GameSave>().FromInstance(_gameSave).AsSingle();
		}

		private void OnApplicationPause(bool _)
		{
			_gameSave.SaveToLocal();
		}

		private void OnApplicationQuit()
		{
			_gameSave.SaveToLocal();
		}
	}
}