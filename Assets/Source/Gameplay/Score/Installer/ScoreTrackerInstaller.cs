using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Score.Installer
{
	public class ScoreTrackerInstaller : MonoInstaller
	{
		[SerializeField] private ScoreTracker _scoreTracker;

		public override void InstallBindings()
		{
			Container.Bind<ScoreTracker>().FromInstance(_scoreTracker).AsSingle();
		}
	}
}