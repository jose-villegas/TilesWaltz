using TilesWalk.Building.Level;
using UnityEngine;
using Zenject;

namespace TilesWalk.Building.Installer
{
	public class TileViewMapInstaller : MonoInstaller
	{
		[SerializeField] private TileViewMap _tileMap;

		public override void InstallBindings()
		{
			Container.Bind<TileViewMap>().FromInstance(_tileMap).AsSingle();
		}
	}
}