using TilesWalk.Gameplay.Persistence;
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
    }
}