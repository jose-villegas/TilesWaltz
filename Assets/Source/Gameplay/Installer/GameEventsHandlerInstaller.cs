using TilesWalk.Gameplay.Input;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Installer
{
	public class GameEventsHandlerInstaller : MonoInstaller
	{
		[SerializeField] private GameEventsHandler _handler;

		public override void InstallBindings()
		{
			Container.Bind<GameEventsHandler>().FromInstance(_handler).AsSingle();
		}
	}
}