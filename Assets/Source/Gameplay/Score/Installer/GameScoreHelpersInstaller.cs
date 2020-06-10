using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Score.Installer
{
	public class GameScoreHelpersInstaller : MonoInstaller
	{
		[SerializeField] private GameScoresHelper _gameScoresHelper;

		public override void InstallBindings()
		{
			Container.Bind<GameScoresHelper>().FromInstance(_gameScoresHelper).AsSingle();
		}
	}
}
