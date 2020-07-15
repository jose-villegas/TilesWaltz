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
                .Delay(TimeSpan.FromSeconds(1.5f))
                .Subscribe(val =>
                {
                    // next dialog
                    _handler.NextStep();
                    _handler.Canvas.DialogContent.OnTextDialogCompletedAsObservable()
                        .Take(1)
                        .Delay(TimeSpan.FromSeconds(2f))
                        .Subscribe(_val =>
                        {
                            // next dialog
                            _handler.NextStep();
                            TileCharacterPointRight();
                            _handler.Canvas.DialogContent.OnTextDialogCompletedAsObservable()
                                .Take(1)
                                .Delay(TimeSpan.FromSeconds(2f))
                                .Subscribe(__val =>
                                {
                                    // next dialog
                                    _handler.NextStep();
                                    TileCharacterPointRight();
                                    _handler.Canvas.DialogContent.OnTextDialogCompletedAsObservable()
                                        .Take(1)
                                        .Delay(TimeSpan.FromSeconds(2f))
                                        .Subscribe(___val =>
                                        {
                                            // next dialog
                                            _handler.NextStep();
                                            TileCharacterPointRight();
                                            _handler.Canvas.DialogContent.OnTextDialogCompletedAsObservable()
                                                .Take(1)
                                                .Delay(TimeSpan.FromSeconds(2f))
                                                .Subscribe(____val =>
                                                {
                                                    // next dialog
                                                    _handler.NextStep();
                                                    TileCharacterPointLeft();
                                                    _handler.Canvas.DialogContent.OnTextDialogCompletedAsObservable()
                                                        .Take(1)
                                                        .Delay(TimeSpan.FromSeconds(2f))
                                                        .Subscribe(_____val =>
                                                        {
                                                            // next dialog
                                                            _handler.NextStep();
                                                            _handler.Canvas.DialogContent
                                                                .OnTextDialogCompletedAsObservable()
                                                                .Take(1)
                                                                .Delay(TimeSpan.FromSeconds(2f))
                                                                .Subscribe(______val =>
                                                                {
                                                                    // next dialog
                                                                    _handler.NextStep();
                                                                    TileCharacterExcited();
                                                                    _handler.Canvas.DialogContent
                                                                        .OnTextDialogCompletedAsObservable()
                                                                        .Take(1)
                                                                        .Delay(TimeSpan.FromSeconds(2f))
                                                                        .Subscribe(_______val =>
                                                                        {
                                                                            // next dialog
                                                                            _handler.NextStep();
                                                                            _handler.Canvas.DialogContent
                                                                                .OnTextDialogCompletedAsObservable()
                                                                                .Take(1)
                                                                                .Delay(TimeSpan.FromSeconds(2f))
                                                                                .Subscribe(________val =>
                                                                                {
                                                                                    _handler.NextStep();
                                                                                    _handler.Canvas.DialogContent
                                                                                        .OnTextDialogCompletedAsObservable()
                                                                                        .Take(1)
                                                                                        .Delay(TimeSpan.FromSeconds(2f))
                                                                                        .Subscribe(_________val =>
                                                                                        {
                                                                                            _handler.NextStep();
                                                                                            _handler.Canvas
                                                                                                .DialogContent
                                                                                                .OnTextDialogCompletedAsObservable()
                                                                                                .Take(1)
                                                                                                .Delay(TimeSpan
                                                                                                    .FromSeconds(2f))
                                                                                                .Subscribe(
                                                                                                    __________val =>
                                                                                                    {
                                                                                                        _handler
                                                                                                            .NextStep();
                                                                                                        TileCharacterPointLeft();
                                                                                                        _handler.Canvas
                                                                                                            .DialogContent
                                                                                                            .OnTextDialogCompletedAsObservable()
                                                                                                            .Take(1)
                                                                                                            .Delay(
                                                                                                                TimeSpan
                                                                                                                    .FromSeconds(
                                                                                                                        2f))
                                                                                                            .Subscribe(
                                                                                                                ___________val =>
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