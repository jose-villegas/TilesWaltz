using TilesWalk.Building;
using UnityEngine;
using Zenject;

namespace TilesWalk.Tile.Installer
{
	public class TileGeneratorInstaller : MonoInstaller
	{
		[SerializeField] private TileGenerator _generator;

		public override void InstallBindings()
		{
			Container.Bind<TileGenerator>().FromInstance(_generator).AsSingle();
		}
	}
}