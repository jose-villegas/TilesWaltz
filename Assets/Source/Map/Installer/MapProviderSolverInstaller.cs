using TilesWalk.Map.General;
using UnityEngine;
using Zenject;

namespace TilesWalk.Map.Installer
{
	public class MapProviderSolverInstaller : MonoInstaller
	{
		[SerializeField] private MapProviderSolver _solver;

		public override void InstallBindings()
		{
			Container.Bind<MapProviderSolver>().FromInstance(_solver);
		}
	}
}