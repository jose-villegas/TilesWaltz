using TilesWalk.Map.Tile;
using UnityEngine;
using Zenject;

namespace TilesWalk.Building.Installer
{
	public class TileViewFactoryInstaller : MonoInstaller
	{
		[SerializeField] private LevelTileViewFactory _levelTileFactory;
		[SerializeField] private GameMapTileFactory _gameMapTileFactory;

		public override void InstallBindings()
		{
			Container.Bind<LevelTileViewFactory>().FromInstance(_levelTileFactory).AsSingle();
			Container.Bind<GameMapTileFactory>().FromInstance(_gameMapTileFactory).AsSingle();
		}
	}
}