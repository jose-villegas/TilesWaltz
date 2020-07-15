using System;
using UniRx;

namespace TilesWalk.Gameplay.Tutorial.Tutorials
{
    public class MapIntroTutorial : TutorialSequencePlayer
    {
        private void Awake()
        {
            if (!_save.Statistics.IsTutorialCompleted("Intro.Map"))
            {
                TriggerSequence();
            }
        }

        public override void PlaySequence()
        {
            _handler.SetupForSequence("Intro.Map").NextStep();
            TileCharacterExcited();
            // next step as soon the first dialogue ends
            _handler.Canvas.DialogContent.OnTextDialogCompletedAsObservable()
                .Take(1)
                .Delay(TimeSpan.FromSeconds(2.5f))
                .Subscribe(val =>
                {
                    // next dialog
                    _handler.NextStep();
                    TileCharacterPointLeft();
                    _handler.Canvas.DialogContent.OnTextDialogCompletedAsObservable()
                        .Take(1)
                        .Delay(TimeSpan.FromSeconds(2.5f))
                        .Subscribe(_val =>
                        {
                            _handler.NextStep();
                            TileCharacterPointLeft();
                            _handler.Canvas.DialogContent.OnTextDialogCompletedAsObservable()
                                .Take(1)
                                .Delay(TimeSpan.FromSeconds(2.5f))
                                .Subscribe(__val =>
                                {
                                    _handler.NextStep();
                                    TileCharacterPointLeft();
                                    _handler.Canvas.DialogContent.OnTextDialogCompletedAsObservable()
                                        .Take(1)
                                        .Delay(TimeSpan.FromSeconds(2.5f))
                                        .Subscribe(___val =>
                                        {
                                            _handler.NextStep();
                                            TileCharacterOrbit();
                                            _handler.Canvas.DialogContent.OnTextDialogCompletedAsObservable()
                                                .Take(1)
                                                .Delay(TimeSpan.FromSeconds(4f))
                                                .Subscribe(____val => { FinishSequence(); }).AddTo(this);
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
            _save.Statistics.CompletedTutorial("Intro.Map");
        }

        private void OnDestroy()
        {
            _save.Statistics.CompletedTutorial("Intro.Map");
        }
    }
}