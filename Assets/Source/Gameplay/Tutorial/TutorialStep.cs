using System;
using System.Collections.Generic;
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
        [SerializeField, ShowIf("_highlight")] private List<int> _identifiers;
        [SerializeField, ShowIf("_highlight")] private bool _bringToFront;
        [SerializeField, ShowIf("_highlight")] private bool _interactable;

        [Header("Animation")] [SerializeField] private TutorialTileCharacter.Gestures _playGesture;

        [SerializeField, Tooltip("Use $ for at end of dialogue, # for just at the beginning, % for after movement completed")]
        private string gestureAtWord;

        [Header("Required Input")] [SerializeField]
        private bool _showDialogActions;

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

        public bool UseBackground
        {
            get => _useBackground;
            set => _useBackground = value;
        }

        public bool BringToFront
        {
            get => _bringToFront;
        }

        public bool Interactable
        {
            get => _interactable;
        }

        public List<int> Identifiers => _identifiers;

        public TutorialTileCharacter.Gestures PlayGesture
        {
            get => _playGesture;
        }

        public bool ShowDialogActions
        {
            get => _showDialogActions;
        }

        public string GestureAtWord
        {
            get => gestureAtWord;
        }

        public TutorialStep(string identifier)
        {
            _stepId = identifier;
        }
    }
}