using System;
using UniRx;

namespace TilesWalk.Gameplay.Tutorial.Tutorials
{
    public class CustomLevelsIntroTutorial : TutorialSequencePlayer
    {
        private void Awake()
        {
            if (!_save.Statistics.IsTutorialCompleted("Intro.CustomLevels"))
            {
                TriggerSequence();
            }
        }

        public override void PlaySequence()
        {
            _handler.SetupForSequence("Intro.CustomLevels").NextStep();
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
                                        .Subscribe(____ => { FinishSequence(); }).AddTo(this);
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
            _save.Statistics.CompletedTutorial("Intro.CustomLevels");
        }

        private void OnDestroy()
        {
            _save.Statistics.CompletedTutorial("Intro.CustomLevels");
        }
    }
}