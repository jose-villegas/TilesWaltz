using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TilesWalk.Gameplay.Tutorial.Tutorials
{
    [RequireComponent(typeof(Button))]
    public class LevelEditorTutorialStarter : TutorialSequencePlayer
    {
        public override void PlaySequence()
        {
            _handler.SetupForSequence("Intro.LevelMaker");
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
                CreateTilesTutorial();
            }).AddTo(this);
        }

        private void CreateTilesTutorial()
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
                DirectionsMatchColorTutorial();
            }).AddTo(this);
        }

        private void DirectionsMatchColorTutorial()
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
                DirectionalDoubleTapTutorial();
            }).AddTo(this);
        }

        private void DirectionalDoubleTapTutorial()
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
                ClickOnTileTutorial();
            }).AddTo(this);
        }

        private void ClickOnTileTutorial()
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
                TapToConfirmTutorial();
            }).AddTo(this);
        }

        private void TapToConfirmTutorial()
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
                OrientationTutorial();
            }).AddTo(this);
        }

        private void OrientationTutorial()
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
                SaveDetailsTutorial();
            }).AddTo(this);
        }

        private void SaveDetailsTutorial()
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
                ShareMapTutorial();
            }).AddTo(this);
        }

        private void ShareMapTutorial()
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
                ImportMapTutorial();
            }).AddTo(this);
        }

        private void ImportMapTutorial()
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
                _handler.FinishSequence();
            }).AddTo(this);
        }
    }
}