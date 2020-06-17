using System.Collections.Generic;
using BayatGames.SaveGameFree;
using TilesWalk.Gameplay.Score;
using Zenject;

namespace TilesWalk.Gameplay.Persistence.Installer
{
	public class GameSaveInstaller : MonoInstaller
	{
		private GameSave _gameSave = new GameSave();

		public override void InstallBindings()
		{
			_gameSave.LoadFromLocal();
			Container.Bind<GameSave>().FromInstance(_gameSave).AsSingle();
		}

		private void OnApplicationQuit()
		{
			_gameSave.SaveToLocal();
		}
	}
}