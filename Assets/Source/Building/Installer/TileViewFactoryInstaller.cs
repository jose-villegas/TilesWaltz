using UnityEngine;
using Zenject;

namespace TilesWalk.Building.Installer
{
	public class TileViewFactoryInstaller : MonoInstaller
	{
		[SerializeField] private TileViewFactory _tileFactory;

		public override void InstallBindings()
		{
			Container.Bind<TileViewFactory>().FromInstance(_tileFactory).AsSingle();
		}
	}
}