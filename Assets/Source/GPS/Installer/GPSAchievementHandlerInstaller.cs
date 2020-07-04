using UnityEngine;
using Zenject;

namespace TilesWalk.GPS.Installer
{
	public class GPSAchievementHandlerInstaller : MonoInstaller
	{
		[SerializeField] private GPSAchievementHandler _handler;

		public override void InstallBindings()
		{
			Container.Bind<GPSAchievementHandler>().FromInstance(_handler).AsSingle();
		}
	}
}