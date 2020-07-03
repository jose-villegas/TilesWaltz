using TilesWalk.Building.Gallery.UI;
using UnityEngine;
using Zenject;

namespace TilesWalk.Building.Gallery.Installer
{
	public class ShareLevelCanvasInstaller : MonoInstaller
	{
		[SerializeField] private ShareLevelCanvas _shareCanvas;

		public override void InstallBindings()
		{
			Container.Bind<ShareLevelCanvas>().FromInstance(_shareCanvas).AsSingle();
		}
	}
}