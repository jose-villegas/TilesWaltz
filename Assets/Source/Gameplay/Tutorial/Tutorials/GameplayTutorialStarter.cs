using System;
using TilesWalk.General.Patterns;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
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
                _slidersInstance.Player.prepareCompleted += OnVideoPrepareCompleted;
            }

            _handler.Canvas.DialogButton.interactable = false;
            // the intro is made of two steps
            _handler.NextStep();

            // enable dialog click after text is completed
            var waitForFill = _handler.Canvas.DialogContent.OnTextDialogFillCompletedAsObservable()
                .Take(1).Subscribe(s => { _handler.Canvas.DialogButton.interactable = true; }).AddTo(this);

            // advise the user to tap after reading
            var waitForRead = _handler.Canvas.DialogContent.OnTextDialogReadCompletedAsObservable()
                .Take(1).Subscribe(s => { _handler.Canvas.TapToContinueCanvas.Show(); }).AddTo(this);

            // configure so click shows the next step
            _handler.Canvas.DialogButton.onClick.AsObservable().Take(1).Subscribe(_ =>
            {
                waitForFill?.Dispose();
                waitForRead?.Dispose();
                ComboTutorial();
            }).AddTo(this);
        }

        private void ComboTutorial()
        {
            _handler.Canvas.DialogButton.interactable = false;
            _handler.Canvas.TapToContinueCanvas.Hide();
            _handler.NextStep();

            // enable dialog click after text is completed
            var waitForFill = _handler.Canvas.DialogContent.OnTextDialogFillCompletedAsObservable()
                .Take(1).Subscribe(s => { _handler.Canvas.DialogButton.interactable = true; }).AddTo(this);

            // advise the user to tap after reading
            var waitForRead = _handler.Canvas.DialogContent.OnTextDialogReadCompletedAsObservable()
                .Take(1).Subscribe(s => { _handler.Canvas.TapToContinueCanvas.Show(); }).AddTo(this);

            // configure so click shows the next step
            _handler.Canvas.DialogButton.onClick.AsObservable().Take(1).Subscribe(_ =>
            {
                waitForFill?.Dispose();
                waitForRead?.Dispose();
                GuidesTutorial();
            }).AddTo(this);
        }

        private void GuidesTutorial()
        {
            _handler.Canvas.DialogButton.interactable = false;
            _handler.Canvas.TapToContinueCanvas.Hide();
            _handler.NextStep();

            // enable dialog click after text is completed
            var waitForFill = _handler.Canvas.DialogContent.OnTextDialogFillCompletedAsObservable()
                .Take(1).Subscribe(s =>
                {
                    _handler.Canvas.DialogButton.interactable = true;
                }).AddTo(this);

            // advise the user to tap after reading
            var waitForRead = _handler.Canvas.DialogContent.OnTextDialogReadCompletedAsObservable()
                .Take(1).Subscribe(s => { _handler.Canvas.TapToContinueCanvas.Show(); }).AddTo(this);

            // configure so click shows the next step
            _handler.Canvas.DialogButton.onClick.AsObservable().Take(1).Subscribe(_ =>
            {
                waitForFill?.Dispose();
                waitForRead?.Dispose();
                DisabledGuidesTutorial();
            }).AddTo(this);
        }

        private void DisabledGuidesTutorial()
        {
            _handler.Canvas.DialogButton.interactable = false;
            _handler.Canvas.TapToContinueCanvas.Hide();
            _handler.NextStep();

            // show next clip
            _slidersInstance.NextClip();

            //// enable dialog click after text is completed
            //var waitForFill = _handler.Canvas.DialogContent.OnTextDialogFillCompletedAsObservable()
            //    .Take(1).Subscribe(s => { _handler.Canvas.DialogButton.interactable = true; }).AddTo(this);

            //// advise the user to tap after reading
            //var waitForRead = _handler.Canvas.DialogContent.OnTextDialogReadCompletedAsObservable()
            //    .Take(1).Subscribe(s => { _handler.Canvas.TapToContinueCanvas.Show(); }).AddTo(this);

            //// configure so click shows the next step
            //_handler.Canvas.DialogButton.onClick.AsObservable().Take(1).Subscribe(_ =>
            //{
            //    waitForFill?.Dispose();
            //    waitForRead?.Dispose();
            //}).AddTo(this);
        }

        private void OnVideoPrepareCompleted(VideoPlayer source)
        {
            source.Play();
        }

        public override void FinishSequence()
        {
        }
    }
}