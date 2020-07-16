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
                .Subscribe(_ =>
                {
                    // next dialog
                    _handler.NextStep();
                    TileCharacterPointLeft();
                    _handler.Canvas.DialogContent.OnTextDialogCompletedAsObservable()
                        .Take(1)
                        .Subscribe(__ =>
                        {
                            _handler.NextStep();
                            TileCharacterPointLeft();
                            _handler.Canvas.DialogContent.OnTextDialogCompletedAsObservable()
                                .Take(1)
                                .Subscribe(___ =>
                                {
                                    _handler.NextStep();
                                    TileCharacterPointLeft();
                                    _handler.Canvas.DialogContent.OnTextDialogCompletedAsObservable()
                                        .Take(1)
                                        .Subscribe(____ =>
                                        {
                                            _handler.NextStep();
                                            TileCharacterOrbit();
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
            _save.Statistics.CompletedTutorial("Intro.Map");
        }

        private void OnDestroy()
        {
            _save.Statistics.CompletedTutorial("Intro.Map");
        }
    }
}