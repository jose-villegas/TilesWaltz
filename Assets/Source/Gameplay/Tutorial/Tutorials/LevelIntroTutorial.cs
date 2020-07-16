using System;
using UniRx;

namespace TilesWalk.Gameplay.Tutorial.Tutorials
{
    public class LevelIntroTutorial : TutorialSequencePlayer
    {
        private void Awake()
        {
            if (!_save.Statistics.IsTutorialCompleted("Intro.Level"))
            {
                TriggerSequence();
            }
        }

        public override void PlaySequence()
        {
            _handler.SetupForSequence("Intro.Level").NextStep();
            TileCharacterExcited();
            // next step as soon the first dialogue ends
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
                            _handler.NextStep();
                            TileCharacterExcited();
                            _handler.Canvas.DialogContent.OnTextDialogCompletedAsObservable()
                                .Take(1)
                                .Subscribe(___ =>
                                {
                                    _handler.NextStep();
                                    _handler.Canvas.DialogContent.OnTextDialogCompletedAsObservable()
                                        .Take(1)
                                        .Subscribe(____ =>
                                        {
                                            _handler.NextStep();
                                            _handler.Canvas.DialogContent.OnTextDialogCompletedAsObservable()
                                                .Take(1)
                                                .Subscribe(_____ => { FinishSequence(); }).AddTo(this);
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
            _save.Statistics.CompletedTutorial("Intro.Level");
        }

        private void OnDestroy()
        {
            _save.Statistics.CompletedTutorial("Intro.Level");
        }
    }
}