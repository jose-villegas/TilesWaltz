using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Tutorial.Installer
{
    public class TutorialSequenceHandlerInstaller : MonoInstaller
    {
        [SerializeField] private TutorialSequenceHandler _handler;

        public override void InstallBindings()
        {
            Container.Bind<TutorialSequenceHandler>().FromInstance(_handler).AsSingle();
        }
    }
}