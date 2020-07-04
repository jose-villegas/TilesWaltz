using UnityEngine;
using Zenject;

namespace TilesWalk.GPGS.Installer
{
	public class GPGSAchievementHandlerInstaller : MonoInstaller
	{
		[SerializeField] private GPGSAchievementHandler _handler;

		public override void InstallBindings()
		{
			Container.Bind<GPGSAchievementHandler>().FromInstance(_handler).AsSingle();
		}
	}
}