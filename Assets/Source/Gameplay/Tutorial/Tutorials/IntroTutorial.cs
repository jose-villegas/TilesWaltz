using System;
using UniRx;

namespace TilesWalk.Gameplay.Tutorial.Tutorials
{
    public class IntroTutorial : TutorialSequencePlayer
    {
        private void Awake()
        {
            if (!_save.Statistics.IsTutorialCompleted("Intro") && _save.Statistics.IsFirstTimeLaunch)
            {
                TriggerSequence();
            }
        }

        public override void PlaySequence()
        {
            _handler.SetupForSequence("Intro").NextStep();
            // next step as soon the first dialogue ends
            _handler.Canvas.DialogContent.OnTextDialogCompletedAsObservable()
                .Take(1)
                .Delay(TimeSpan.FromSeconds(1.5f))
                .Subscribe(val =>
                {
                    _handler.NextStep();
                    TileCharacterPointer();

                    // handle click action
                    _handler.TileCharacter.OnTileCharacterClickedAsObservable().Take(1).Subscribe(_val =>
                    {
                        _handler.TileCharacter.ToggleGesture(TutorialTileCharacter.Gestures.Excited);
                        TileCharacterPointLeft();
                        // show next dialog
                        _handler.NextStep();
                        _handler.Canvas.DialogContent.OnTextDialogCompletedAsObservable()
                            .Take(1)
                            .Delay(TimeSpan.FromSeconds(2.5f))
                            .Subscribe(__val =>
                            {
                                TileCharacterPointLeft();
                                // show next dialog
                                _handler.NextStep();
                                _handler.Canvas.DialogContent.OnTextDialogCompletedAsObservable()
                                    .Take(1)
                                    .Delay(TimeSpan.FromSeconds(2.5f))
                                    .Subscribe(___val =>
                                    {
                                        TileCharacterPointLeft();
                                        // show last dialog, on completion call Finish
                                        _handler.NextStep();
                                        _handler.Canvas.DialogContent.OnTextDialogCompletedAsObservable()
                                            .Take(1)
                                            .Delay(TimeSpan.FromSeconds(2f))
                                            .Subscribe(____val => FinishSequence());
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
            _save.Statistics.CompletedTutorial("Intro");
            _handler.TileCharacter.OnTileCharacterClickedAsObservable().Subscribe(val =>
            {
                _handler.TileCharacter.ToggleGesture(TutorialTileCharacter.Gestures.Excited);
            }).AddTo(this);
        }

        private void OnDestroy()
        {
            _save.Statistics.CompletedTutorial("Intro");
        }
    }
}