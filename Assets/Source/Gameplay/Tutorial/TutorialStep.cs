using System;
using NaughtyAttributes;
using UnityEngine;

namespace TilesWalk.Gameplay.Tutorial
{
    [Serializable]
    public class TutorialStep
    {
        [SerializeField] private string _stepId;
        [SerializeField] private string _message;
        [SerializeField] private Vector3 _characterPosition;
        [SerializeField] private bool _useBackground;

        [Header("Highlight")] [SerializeField] private bool _highlight;
        [SerializeField, ShowIf("_highlight")] private int _identifier;
        [SerializeField, ShowIf("_highlight")] private bool _bringToFront;
        [SerializeField, ShowIf("_highlight")] private bool _interactable;
        public string StepId => _stepId;

        public string Message
        {
            get => _message;
            set => _message = value;
        }

        public Vector3 CharacterPosition
        {
            get => _characterPosition;
            set => _characterPosition = value;
        }

        public bool Highlight
        {
            get => _highlight;
            set => _highlight = value;
        }

        public int Identifier
        {
            get => _identifier;
            set => _identifier = value;
        }

        public bool UseBackground
        {
            get => _useBackground;
            set => _useBackground = value;
        }

        public bool BringToFront
        {
            get => _bringToFront;
            set => _bringToFront = value;
        }

        public bool Interactable
        {
            get => _interactable;
            set => _interactable = value;
        }

        public TutorialStep(string identifier)
        {
            _stepId = identifier;
        }
    }
}