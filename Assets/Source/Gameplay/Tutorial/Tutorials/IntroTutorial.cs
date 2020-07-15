using System;
using UniRx;

namespace TilesWalk.Gameplay.Tutorial.Tutorials
{
    public class IntroTutorial : TutorialSequencePlayer
    {
        private void Awake()
        {
            TriggerSequence();
        }

        public override void PlaySequence()
        {
            _handler.SetupForSequence("Intro").NextStep();
            // next step as soon the first dialogue ends
            _handler.Canvas.DialogContent.OnTextDialogCompletedAsObservable()
                .Take(1)
                .Delay(TimeSpan.FromSeconds(1.5f))
                .Subscribe(val => { _handler.NextStep(); }).AddTo(this);
            // handle click action
            _handler.TileCharacter.OnTileCharacterClickedAsObservable().Take(1).Subscribe(val =>
            {
                if (_handler.CurrentStepIndex == 2)
                {
                    // show next dialog
                    _handler.NextStep();
                    _handler.Canvas.DialogContent.OnTextDialogCompletedAsObservable()
                        .Take(1)
                        .Delay(TimeSpan.FromSeconds(2.5f))
                        .Subscribe(_val =>
                        {
                            // show next dialog
                            _handler.NextStep();
                            _handler.Canvas.DialogContent.OnTextDialogCompletedAsObservable()
                                .Take(1)
                                .Delay(TimeSpan.FromSeconds(2.5f))
                                .Subscribe(__val =>
                                {
                                    // show last dialog, on completion call Finish
                                    _handler.NextStep();
                                    _handler.Canvas.DialogContent.OnTextDialogCompletedAsObservable()
                                        .Take(1)
                                        .Delay(TimeSpan.FromSeconds(2f))
                                        .Subscribe(___val => FinishSequence());
                                }).AddTo(this);
                        }).AddTo(this);
                }
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
        }
    }
}