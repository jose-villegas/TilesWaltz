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

        public abstract void FinishSequence();
    }
}