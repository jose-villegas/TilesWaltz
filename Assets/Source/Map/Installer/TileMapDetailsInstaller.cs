using TilesWalk.Navigation.UI;
using UnityEngine;
using Zenject;

namespace TilesWalk.Navigation.Installer
{
	public class TileMapDetailsInstaller : MonoInstaller
	{
		[SerializeField] private TileMapDetailsCanvas _tileMapDetailsCanvas;

		public override void InstallBindings()
		{
			Container.Bind<TileMapDetailsCanvas>().FromInstance(_tileMapDetailsCanvas).AsSingle();
		}
	}
}