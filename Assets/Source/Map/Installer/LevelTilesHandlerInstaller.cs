using TilesWalk.Map.Tile;
using TilesWalk.Navigation.UI;
using UnityEngine;
using Zenject;

namespace TilesWalk.Navigation.Installer
{
	public class LevelTilesHandlerInstaller : MonoInstaller
	{
		[SerializeField] private GameLevelTilesInitializer _gameLevelTilesInitializer;

		public override void InstallBindings()
		{
			Container.Bind<GameLevelTilesInitializer>().FromInstance(_gameLevelTilesInitializer).AsSingle();
		}
	}
}