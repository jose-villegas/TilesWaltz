using UnityEngine;
using Zenject;

namespace TilesWalk.Building.Installer
{
	public class TileViewFactoryInstaller : MonoInstaller
	{
		[SerializeField] private LevelTileViewFactory _levelTileFactory;

		public override void InstallBindings()
		{
			Container.Bind<LevelTileViewFactory>().FromInstance(_levelTileFactory).AsSingle();
		}
	}
}