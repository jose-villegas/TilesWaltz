using TilesWalk.Map.Bridge;
using UnityEngine;
using Zenject;

namespace TilesWalk.Navigation.Installer
{
	public class MapLevelBridgeInstaller : MonoInstaller
	{
		[SerializeField] private LevelBridge _levelBridge;

		public override void InstallBindings()
		{
			Container.Bind<LevelBridge>().FromInstance(_levelBridge).AsSingle();
		}
	}
}