using TilesWalk.General.Patterns;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
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
            _handler.Canvas.ConfigureDialogActions(OnConfirmClick, OnCancelClick);
        }

        private void OnCancelClick()
        {
            _handler.FinishSequence();
        }

        private void OnConfirmClick()
        {
           
        }

        public override void FinishSequence()
        {
        }
    }
}