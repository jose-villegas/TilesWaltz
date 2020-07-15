using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace TilesWalk.Gameplay.Tutorial
{
    public class TutorialTileCharacter : ObservableTriggerBase
    {
        public enum Gestures
        {
            ShowPointer,
            Excited,
            PointLeft,
            Orbit
        }

        private Subject<TutorialTileCharacter> _onTileCharacterClicked;
        private Animator _animator;

        private Animator Animator
        {
            get
            {
                if (_animator == null)
                {
                    _animator = GetComponentInParent<Animator>();
                }

                return _animator;
            }
        }

        private void OnMouseDown()
        {
            _onTileCharacterClicked?.OnNext(this);
        }

        public IObservable<TutorialTileCharacter> OnTileCharacterClickedAsObservable()
        {
            return _onTileCharacterClicked == null
                ? _onTileCharacterClicked = new Subject<TutorialTileCharacter>()
                : _onTileCharacterClicked;
        }

        protected override void RaiseOnCompletedOnDestroy()
        {
            _onTileCharacterClicked?.OnCompleted();
        }

        public void ToggleGesture(Gestures gesture)
        {
            switch (gesture)
            {
                case Gestures.ShowPointer:
                    Animator.SetTrigger("Pointer");
                    break;
                case Gestures.Excited:
                    Animator.SetTrigger("Excited");
                    break;
                case Gestures.PointLeft:
                    Animator.SetTrigger("PointLeft");
                    break;
                case Gestures.Orbit:
                    Animator.SetTrigger("Orbit");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gesture), gesture, null);
            }
        }
    }
}