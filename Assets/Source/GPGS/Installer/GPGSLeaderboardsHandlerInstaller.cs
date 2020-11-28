using UnityEngine;
using Zenject;

namespace TilesWalk.GPGS.Installer
{
	public class GPGSLeaderboardsHandlerInstaller : MonoInstaller
	{
		[SerializeField] private GPGSLeaderboardsHandler _handler;

		public override void InstallBindings()
		{
			Container.Bind<GPGSLeaderboardsHandler>().FromInstance(_handler).AsSingle();
		}
	}
}