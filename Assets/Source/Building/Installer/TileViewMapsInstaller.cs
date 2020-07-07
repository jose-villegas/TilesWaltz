using TilesWalk.Building.Level;
using TilesWalk.Map.Tile;
using UnityEngine;
using Zenject;

namespace TilesWalk.Building.Installer
{
	public class TileViewMapsInstaller : MonoInstaller
	{
		[SerializeField] private TileViewLevelMap _tileLevelMap;
		[SerializeField] private GameLevelsMapBuilder _gameMapBuilder;

		public override void InstallBindings()
		{
			Container.Bind<TileViewLevelMap>().FromInstance(_tileLevelMap).AsSingle();
			Container.Bind<GameLevelsMapBuilder>().FromInstance(_gameMapBuilder).AsSingle();
		}
	}
}