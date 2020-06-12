using TilesWalk.Building.Level;
using UnityEngine;
using Zenject;

namespace TilesWalk.Building.Installer
{
	public class TileViewLevelMapInstaller : MonoInstaller
	{
		[SerializeField] private TileViewLevelMap _tileLevelMap;

		public override void InstallBindings()
		{
			Container.Bind<TileViewLevelMap>().FromInstance(_tileLevelMap).AsSingle();
		}
	}
}