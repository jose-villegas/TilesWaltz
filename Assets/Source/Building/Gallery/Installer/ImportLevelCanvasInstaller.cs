using TilesWalk.Building.Gallery.UI;
using UnityEngine;
using Zenject;

namespace TilesWalk.Building.Gallery.Installer
{
	public class ImportLevelCanvasInstaller : MonoInstaller
	{
		[SerializeField] private ImportLevelCanvas _importCanvas;

		public override void InstallBindings()
		{
			Container.Bind<ImportLevelCanvas>().FromInstance(_importCanvas).AsSingle();
		}
	}
}