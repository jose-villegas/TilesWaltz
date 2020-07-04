using UnityEngine;
using Zenject;

namespace TilesWalk.GPGS.Installer
{
	public class GPGSLeaderboardsHandlerInstaller : MonoInstaller
	{
		[SerializeField] private GPGSLeaderbardsHandler _handler;

		public override void InstallBindings()
		{
			Container.Bind<GPGSLeaderbardsHandler>().FromInstance(_handler).AsSingle();
		}
	}
}