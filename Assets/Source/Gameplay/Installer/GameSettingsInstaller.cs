using TilesWalk.Gameplay.Animation;
using TilesWalk.Gameplay.Display;
using TilesWalk.Gameplay.Level;
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
		[SerializeField] private CustomLevelsConfiguration _customLevelsSettings;
		[SerializeField] private GameTileColorsConfiguration _gamePalette;

		public GameTileColorsConfiguration GamePalette => _gamePalette;

		public override void InstallBindings()
		{
			Container.Bind<ScorePointsConfiguration>().FromInstance(_scorePointsSettings).AsSingle();
			Container.Bind<AnimationConfiguration>().FromInstance(_animationSettings).AsSingle();
			Container.Bind<GameTileColorsConfiguration>().FromInstance(_gamePalette).AsSingle();
			Container.Bind<CustomLevelsConfiguration>().FromInstance(_customLevelsSettings).AsSingle();
		}
	}
}