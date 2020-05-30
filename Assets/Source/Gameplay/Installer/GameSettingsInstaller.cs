using TilesWalk.Gameplay;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "Installers/GameSettingsInstaller")]
public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
{
    [SerializeField]
	private ScoreConfiguration _scoreSettings;
	[SerializeField]
	private AnimationConfiguration _animationSettings;

    public override void InstallBindings()
    {
	    Container.Bind<ScoreConfiguration>().FromInstance(_scoreSettings).AsSingle();
	    Container.Bind<AnimationConfiguration>().FromInstance(_animationSettings).AsSingle();
	}
}