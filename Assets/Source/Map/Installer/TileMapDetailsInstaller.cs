using TilesWalk.Navigation.UI;
using UnityEngine;
using Zenject;

namespace TilesWalk.Navigation.Installer
{
	public class TileMapDetailsInstaller : MonoInstaller
	{
		[SerializeField] private LevelMapDetailsCanvas _levelMapDetailsCanvas;

		public override void InstallBindings()
		{
			Container.Bind<LevelMapDetailsCanvas>().FromInstance(_levelMapDetailsCanvas).AsSingle();
		}
	}
}