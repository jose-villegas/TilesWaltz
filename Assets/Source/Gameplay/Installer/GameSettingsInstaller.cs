using TilesWalk.Gameplay;
using TilesWalk.Gameplay.Animation;
using TilesWalk.Gameplay.Score;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "Installers/GameSettingsInstaller")]
public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
{
    [SerializeField]
	private ScorePointsConfiguration _scorePointsSettings;
	[SerializeField]
	private AnimationConfiguration _animationSettings;

    public override void InstallBindings()
    {
	    Container.Bind<ScorePointsConfiguration>().FromInstance(_scorePointsSettings).AsSingle();
	    Container.Bind<AnimationConfiguration>().FromInstance(_animationSettings).AsSingle();
	}
}