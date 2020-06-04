using TilesWalk.Navigation.UI;
using UnityEngine;
using Zenject;

namespace TilesWalk.Navigation.Installer
{
	public class TileMapDetailsInstaller : MonoInstaller
	{
		[SerializeField] private TileMapDetails _tileMapDetails;

		public override void InstallBindings()
		{
			Container.Bind<TileMapDetails>().FromInstance(_tileMapDetails).AsSingle();
		}
	}
}