using TilesWalk.General.UI;
using UnityEngine;
using Zenject;

namespace TilesWalk.General.Installer
{
	public class CanvasHoverListenerInstaller : MonoInstaller
	{
		[SerializeField] private CanvasHoverListener _listener;

		public override void InstallBindings()
		{
			Container.Bind<CanvasHoverListener>().FromInstance(_listener).AsSingle();
		}
	}
}