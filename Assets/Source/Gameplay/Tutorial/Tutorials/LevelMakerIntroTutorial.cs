using System;
using UniRx;

namespace TilesWalk.Gameplay.Tutorial.Tutorials
{
    public class LevelMakerIntroTutorial : TutorialSequencePlayer
    {
        private void Awake()
        {
            if (!_save.Statistics.IsTutorialCompleted("Intro.LevelMaker"))
            {
                TriggerSequence();
            }
        }

        public override void PlaySequence()
        {
            _handler.SetupForSequence("Intro.LevelMaker").NextStep();
            TileCharacterExcited();
            // next step as soon the first dialogue ends
            // todo: handle this horrible nesting better
            _handler.Canvas.DialogContent.OnTextDialogCompletedAsObservable()
                .Take(1)
                .Subscribe(_ =>
                {
                    // next dialog
                    _handler.NextStep();
                    _handler.Canvas.DialogContent.OnTextDialogCompletedAsObservable()
                        .Take(1)
                        .Subscribe(__ =>
                        {
                            // next dialog
                            _handler.NextStep();
                            TileCharacterPointRight();
                            _handler.Canvas.DialogContent.OnTextDialogCompletedAsObservable()
                                .Take(1)
                                .Subscribe(___ =>
                                {
                                    // next dialog
                                    _handler.NextStep();
                                    TileCharacterPointRight();
                                    _handler.Canvas.DialogContent.OnTextDialogCompletedAsObservable()
                                        .Take(1)
                                        .Subscribe(____ =>
                                        {
                                            // next dialog
                                            _handler.NextStep();
                                            TileCharacterPointRight();
                                            _handler.Canvas.DialogContent.OnTextDialogCompletedAsObservable()
                                                .Take(1)
                                                .Subscribe(_____ =>
                                                {
                                                    // next dialog
                                                    _handler.NextStep();
                                                    TileCharacterPointLeft();
                                                    _handler.Canvas.DialogContent.OnTextDialogCompletedAsObservable()
                                                        .Take(1)
                                                        .Subscribe(______ =>
                                                        {
                                                            // next dialog
                                                            _handler.NextStep();
                                                            _handler.Canvas.DialogContent
                                                                .OnTextDialogCompletedAsObservable()
                                                                .Take(1)
                                                                .Subscribe(_______ =>
                                                                {
                                                                    // next dialog
                                                                    _handler.NextStep();
                                                                    TileCharacterExcited();
                                                                    _handler.Canvas.DialogContent
                                                                        .OnTextDialogCompletedAsObservable()
                                                                        .Take(1)
                                                                        .Subscribe(________ =>
                                                                        {
                                                                            // next dialog
                                                                            _handler.NextStep();
                                                                            _handler.Canvas.DialogContent
                                                                                .OnTextDialogCompletedAsObservable()
                                                                                .Take(1)
                                                                                .Subscribe(_________ =>
                                                                                {
                                                                                    _handler.NextStep();
                                                                                    _handler.Canvas.DialogContent
                                                                                        .OnTextDialogCompletedAsObservable()
                                                                                        .Take(1)
                                                                                        .Subscribe(__________ =>
                                                                                        {
                                                                                            _handler.NextStep();
                                                                                            _handler.Canvas
                                                                                                .DialogContent
                                                                                                .OnTextDialogCompletedAsObservable()
                                                                                                .Take(1)
                                                                                                .Subscribe(
                                                                                                    ___________ =>
                                                                                                    {
                                                                                                        _handler
                                                                                                            .NextStep();
                                                                                                        TileCharacterPointLeft();
                                                                                                        _handler.Canvas
                                                                                                            .DialogContent
                                                                                                            .OnTextDialogCompletedAsObservable()
                                                                                                            .Take(1)
                                                                                                            .Subscribe(
                                                                                                                ____________ =>
                                                                                                                {
                                                                                                                    FinishSequence();
                                                                                                                })
                                                                                                            .AddTo(
                                                                                                                this);
                                                                                                    })
                                                                                                .AddTo(this);
                                                                                        })
                                                                                        .AddTo(this);
                                                                                })
                                                                                .AddTo(this);
                                                                        })
                                                                        .AddTo(this);
                                                                })
                                                                .AddTo(this);
                                                        }).AddTo(this);
                                                }).AddTo(this);
                                        }).AddTo(this);
                                }).AddTo(this);
                        }).AddTo(this);
                }).AddTo(this);
        }

        public override void TriggerSequence()
        {
            PlaySequence();
        }

        public override void FinishSequence()
        {
            _handler.FinishSequence();
            _handler.Canvas.Hide();
            _save.Statistics.CompletedTutorial("Intro.LevelMaker");
        }

        private void OnDestroy()
        {
            _save.Statistics.CompletedTutorial("Intro.LevelMaker");
        }
    }
}