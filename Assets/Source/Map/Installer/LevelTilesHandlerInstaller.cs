using TilesWalk.Map.Tile;
using TilesWalk.Navigation.UI;
using UnityEngine;
using Zenject;

namespace TilesWalk.Navigation.Installer
{
	public class LevelTilesHandlerInstaller : MonoInstaller
	{
		[SerializeField] private LevelTilesHandler _levelTilesHandler;

		public override void InstallBindings()
		{
			Container.Bind<LevelTilesHandler>().FromInstance(_levelTilesHandler).AsSingle();
		}
	}
}