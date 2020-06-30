using TilesWalk.Map.Tile;
using TilesWalk.Tile;
using UnityEngine;
using Zenject;

namespace TilesWalk.General.Installer
{
	public class MaterialColorMatchHandlersInstaller : MonoInstaller
	{
		[SerializeField] private TileColorMaterialColorMatchHandler _tileColorMaterialHandler;
		[SerializeField] private LevelStateTileMaterialHandler _levelStateMaterialHandler;
		[SerializeField] private LevelStarsTileMaterialHandler _levelStarsMaterialHandler;

		public override void InstallBindings()
		{
			Container.Bind<TileColorMaterialColorMatchHandler>().FromInstance(_tileColorMaterialHandler).AsSingle();
			Container.Bind<LevelStateTileMaterialHandler>().FromInstance(_levelStateMaterialHandler).AsSingle();
			Container.Bind<LevelStarsTileMaterialHandler>().FromInstance(_levelStarsMaterialHandler).AsSingle();
		}
	}
}