using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace TilesWalk.Installers.Tile
{
	public class TileInstaller : MonoInstaller
	{
		[SerializeField]
		private GameObject _tileAsset;

		public override void InstallBindings()
		{
			Container.Bind<GameObject>().WithId("TileAsset").FromInstance(_tileAsset).AsSingle();
		}
	}
}

