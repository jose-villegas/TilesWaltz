using TilesWalk.Building.Map;
using TilesWalk.Navigation.Map;
using UnityEngine;
using Zenject;

namespace TilesWalk.Navigation.Installer
{
	public class MapLevelBridgeInstaller : MonoInstaller
	{
		[SerializeField] private MapLevelBridge _mapLevelBridge;

		public override void InstallBindings()
		{
			Container.Bind<MapLevelBridge>().FromInstance(_mapLevelBridge).AsSingle();
		}
	}
}