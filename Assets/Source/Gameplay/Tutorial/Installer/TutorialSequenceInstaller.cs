using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Tutorial.Installer
{
    [CreateAssetMenu(fileName = "TutorialSequenceInstaller", menuName = "Installers/TutorialSequenceInstaller")]
    public class TutorialSequenceInstaller :  ScriptableObjectInstaller<TutorialSequenceInstaller>
    {
        [SerializeField] private List<TutorialSequence> _gameTutorials;

        public List<TutorialSequence> GameTutorials
        {
            get => _gameTutorials;
            set => _gameTutorials = value;
        }

        public override void InstallBindings()
        {
            Container.Bind<List<TutorialSequence>>().WithId("GameTutorials").FromInstance(_gameTutorials).AsSingle();
        }
    }
}