using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Score.Installer
{
	public class LevelScoreTrackerInstaller : MonoInstaller
	{
		[SerializeField] private LevelScoreTracker _levelScoreTracker;

		public override void InstallBindings()
		{
			Container.Bind<LevelScoreTracker>().FromInstance(_levelScoreTracker).AsSingle();
		}
	}
}