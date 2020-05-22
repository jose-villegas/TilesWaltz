using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace TilesWalk.Installers.Tile
{
	public class TileInstaller : MonoInstaller
	{
		[SerializeField]
		private AssetReference _tileAsset;

		public override void InstallBindings()
		{
			Container.Bind<AssetReference>().WithId("TileAsset").FromInstance(_tileAsset).AsSingle();
		}
	}
}

