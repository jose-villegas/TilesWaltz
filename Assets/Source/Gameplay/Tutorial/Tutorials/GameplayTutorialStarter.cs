using TilesWalk.General.Patterns;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace TilesWalk.Gameplay.Tutorial.Tutorials
{
    [RequireComponent(typeof(Button))]
    public class GameplayTutorialStarter : TutorialSequencePlayer
    {
        [Inject] private DiContainer _container;

        [SerializeField] private GameplayVideoSlides _sliders;

        private GameplayVideoSlides _slidersInstance;

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
            _handler.Canvas.DialogActions.Hide();
            _handler.FinishSequence();
        }

        private void OnConfirmClick()
        {
            var content = _handler.Canvas.ContentContainer;

            if (_slidersInstance == null)
            {
                var instance = _container.InstantiatePrefab(_sliders, content.transform);

                _slidersInstance = instance.GetComponent<GameplayVideoSlides>();
                _slidersInstance.NextButton.interactable = false;
                _slidersInstance.Player.Play();
            }

            _handler.Canvas.DialogButton.interactable = false;
            // the intro is made of two steps
            _handler.NextStep();

            // enable dialog click after text is completed
            _handler.Canvas.DialogContent.OnTextDialogFillCompletedAsObservable()
                .Take(1).Subscribe(s => { _handler.Canvas.DialogButton.interactable = true; }).AddTo(this);

            // advise the user to tap after reading
            _handler.Canvas.DialogContent.OnTextDialogReadCompletedAsObservable()
                .Take(1).Subscribe(s => { _handler.Canvas.TapToContinueCanvas.Show(); }).AddTo(this);

            // configure so click shows the next step
            _handler.Canvas.DialogButton.onClick.AsObservable()
                .Take(1).Subscribe(unit =>
                {
                    _handler.Canvas.TapToContinueCanvas.Hide();
                    _handler.NextStep();
                }).AddTo(this);
        }

        public override void FinishSequence()
        {
        }
    }
}