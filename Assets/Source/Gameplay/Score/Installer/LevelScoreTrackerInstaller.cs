using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Score.Installer
{
	public class LevelScoreTrackerInstaller : MonoInstaller
	{
		[SerializeField] private LevelScorePointsTracker _levelScorePointsTracker;

		public override void InstallBindings()
		{
			Container.Bind<LevelScorePointsTracker>().FromInstance(_levelScorePointsTracker).AsSingle();
		}
	}
}