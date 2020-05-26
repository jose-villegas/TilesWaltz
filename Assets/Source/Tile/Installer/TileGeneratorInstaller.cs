using TilesWalk.Building;
using UnityEngine;
using Zenject;

namespace TilesWalk.Tile.Installer
{
	public class TileGeneratorInstaller : MonoInstaller
	{
		[SerializeField] private TileViewFactory _viewFactory;

		public override void InstallBindings()
		{
			Container.Bind<TileViewFactory>().FromInstance(_viewFactory).AsSingle();
		}
	}
}