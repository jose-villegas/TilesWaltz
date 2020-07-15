using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace TilesWalk.Gameplay.Tutorial
{
    [Serializable]
    public class TutorialSequence
    {
        [SerializeField] private string _sequenceId;
        [SerializeField] private List<TutorialStep> _steps;
        [SerializeField] private bool _hideCharacterAfter;

        [SerializeField, HideIf("_hideCharacterAfter")]
        private List<string> _possiblePhrases;

        [SerializeField, HideIf("_hideCharacterAfter")]
        private Vector3 _lastPosition;

        public string SequenceId => _sequenceId;

        public List<TutorialStep> Steps => _steps;

        public List<string> PossiblePhrases => _possiblePhrases;

        public bool HideCharacterAfter => _hideCharacterAfter;

        public Vector3 LastPosition => _lastPosition;

        public TutorialSequence(string id)
        {
            _sequenceId = id;
            _steps = new List<TutorialStep>();
        }
    }
}