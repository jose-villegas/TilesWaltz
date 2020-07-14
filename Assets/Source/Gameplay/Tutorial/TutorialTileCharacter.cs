using System;
using UniRx;
using UniRx.Triggers;

namespace TilesWalk.Gameplay.Tutorial
{
    public class TutorialTileCharacter : ObservableTriggerBase
    {
        private Subject<TutorialTileCharacter> _onTileCharacterClicked;

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
    }
}