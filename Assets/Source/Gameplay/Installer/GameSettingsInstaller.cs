using TilesWalk.Gameplay.Animation;
using TilesWalk.Gameplay.Display;
using TilesWalk.Gameplay.Score;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Installer
{
	[CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "Installers/GameSettingsInstaller")]
	public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
	{
		[SerializeField] private ScorePointsConfiguration _scorePointsSettings;
		[SerializeField] private AnimationConfiguration _animationSettings;
		[SerializeField] private GameTileColorsConfiguration _tileColorsSettings;

		public override void InstallBindings()
		{
			Container.Bind<ScorePointsConfiguration>().FromInstance(_scorePointsSettings).AsSingle();
			Container.Bind<AnimationConfiguration>().FromInstance(_animationSettings).AsSingle();
			Container.Bind<GameTileColorsConfiguration>().FromInstance(_tileColorsSettings).AsSingle();
		}
	}
}