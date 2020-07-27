using TilesWalk.General.Patterns;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace TilesWalk.Gameplay.Tutorial.Tutorials
{
    [RequireComponent(typeof(Button))]
    public class GameplayTutorialStarter : TutorialSequencePlayer
    {
        private void Awake()
        {
            var button = GetComponent<Button>();

            button.onClick.AsObservable().Subscribe(unit => PlaySequence()).AddTo(this);
        }

        public override void PlaySequence()
        {
            _handler.SetupForSequence("Intro.Level");
            _handler.NextStep();
        }

        public override void FinishSequence()
        {
        }
    }
}