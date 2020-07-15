using System;
using TilesWalk.Gameplay.Persistence;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Tutorial
{
    public abstract class TutorialSequencePlayer : MonoBehaviour
    {
        [Inject] protected TutorialSequenceHandler _handler;
        [Inject] protected GameSave _save;

        public abstract void PlaySequence();
        public abstract void TriggerSequence();

        public abstract void FinishSequence();

        protected void TileCharacterPointer()
        {
            _handler.TileCharacter.ToggleGesture(TutorialTileCharacter.Gestures.ShowPointer);
        }

        protected void TileCharacterExcited()
        {
            Observable.Timer(TimeSpan.FromSeconds(.5f)).Subscribe(_ => { },
                    () => { _handler.TileCharacter.ToggleGesture(TutorialTileCharacter.Gestures.Excited); })
                .AddTo(this);
        }

        protected void TileCharacterOrbit()
        {
            Observable.Timer(TimeSpan.FromSeconds(.5f)).Subscribe(_ => { },
                    () => { _handler.TileCharacter.ToggleGesture(TutorialTileCharacter.Gestures.Orbit); })
                .AddTo(this);
        }

        protected void TileCharacterPointLeft()
        {
            Observable.Timer(TimeSpan.FromSeconds(.5f)).Subscribe(_ => { },
                    () => { _handler.TileCharacter.ToggleGesture(TutorialTileCharacter.Gestures.PointLeft); })
                .AddTo(this);
        }

        protected void TileCharacterPointRight()
        {
            Observable.Timer(TimeSpan.FromSeconds(.5f)).Subscribe(_ => { },
                    () => { _handler.TileCharacter.ToggleGesture(TutorialTileCharacter.Gestures.PointRight); })
                .AddTo(this);
        }
    }
}