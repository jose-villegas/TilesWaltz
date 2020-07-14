using System;
using System.Collections.Generic;
using UnityEngine;

namespace TilesWalk.Gameplay.Tutorial
{
    [Serializable]
    public class TutorialSequence
    {
        [SerializeField] private string _sequenceId;
        [SerializeField] private List<TutorialStep> _steps;

        public string SequenceId => _sequenceId;

        public List<TutorialStep> Steps => _steps;

        public TutorialSequence(string id)
        {
            _sequenceId = id;
            _steps = new List<TutorialStep>();
        }
    }
}