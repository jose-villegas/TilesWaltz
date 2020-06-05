using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Condition.Installer
{
	public class LevelFinishTrackerInstaller : MonoInstaller
	{
		[SerializeField] private LevelFinishTracker _levelFinishTracker;

		public override void InstallBindings()
		{
			Container.Bind<LevelFinishTracker>().FromInstance(_levelFinishTracker).AsSingle();
		}
	}
}