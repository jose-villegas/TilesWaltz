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
    public class GameplayTutorialStarter : TutorialSequencePlayer
    {
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

            InstanceTutorialVideoSlider(content);

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
                .Take(1).Subscribe(s => { _handler.Canvas.DialogButton.interactable = true; }).AddTo(this);

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
            SlidersInstance.NextClip();

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
                DirectionPowerUpTutorial();
            }).AddTo(this);
        }

        private void DirectionPowerUpTutorial()
        {
            _handler.Canvas.DialogButton.interactable = false;
            _handler.Canvas.TapToContinueCanvas.Hide();
            _handler.NextStep();

            // show next clip
            SlidersInstance.NextClip();

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
                ColorMatchPowerUpTutorial();
            }).AddTo(this);
        }

        private void ColorMatchPowerUpTutorial()
        {
            _handler.Canvas.DialogButton.interactable = false;
            _handler.Canvas.TapToContinueCanvas.Hide();
            _handler.NextStep();

            // show next clip
            SlidersInstance.NextClip();

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
                FinishSequence();
            }).AddTo(this);
        }

        public override void FinishSequence()
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
                Destroy(SlidersInstance.gameObject);
                _handler.FinishSequence();
            }).AddTo(this);
        }
    }
}