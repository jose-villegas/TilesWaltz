using System.Collections.Generic;
using NaughtyAttributes;
using TilesWalk.Gameplay.Tutorial.UI;
using TilesWalk.General.UI;
using TilesWalk.Gameplay.Tutorial.Installer;
using UnityEngine;
using Zenject;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TilesWalk.Gameplay.Tutorial
{
    public class TutorialSequenceHandler : MonoBehaviour
    {
        [Inject(Id = "GameTutorials"), SerializeField]
        private List<TutorialSequence> _gameTutorials;

        /// <summary>
        /// The tile talking character
        /// </summary>
        [SerializeField] private TutorialTileCharacter _tileCharacter;

        private TutorialCanvas _canvas;

        private TutorialCanvas Canvas
        {
            get
            {
                if (_canvas == null)
                {
                    _canvas = GetComponentInChildren<TutorialCanvas>();
                }

                return _canvas;
            }
        }

        /// <summary>
        /// The tile talking character
        /// </summary>
        public TutorialTileCharacter TileCharacter => _tileCharacter;

#if UNITY_EDITOR
        [Button(enabledMode: EButtonEnableMode.Editor)]
        private void LoadTutorials()
        {
            if (_gameTutorials == null)
            {
                var tutorials = AssetDatabase.LoadAssetAtPath("Assets/Resources/TutorialSequenceInstaller.asset",
                    typeof(TutorialSequenceInstaller)) as TutorialSequenceInstaller;

                if (tutorials != null) _gameTutorials = tutorials.GameTutorials;
            }
        }

        [Header("Sequence Building - Editor"), SerializeField]
        private string _sequenceId;

        [SerializeField] private string _stepId;
        [SerializeField] private RectTransform _highlight;

        [Button(enabledMode: EButtonEnableMode.Editor)]
        private void AddStep()
        {
            LoadTutorials();

            var tutorials = AssetDatabase.LoadAssetAtPath("Assets/Resources/TutorialSequenceInstaller.asset",
                typeof(TutorialSequenceInstaller)) as TutorialSequenceInstaller;

            // check if this sequence already exissts
            var indexOf = _gameTutorials.FindIndex(x => x.SequenceId == _sequenceId);

            var sequence = indexOf >= 0 ? _gameTutorials[indexOf] : new TutorialSequence(_sequenceId);

            if (indexOf < 0) _gameTutorials.Add(sequence);

            // now check for steps
            indexOf = sequence.Steps.FindIndex(x => x.StepId == _stepId);

            var step = indexOf >= 0 ? sequence.Steps[indexOf] : new TutorialStep(_stepId);

            step.CharacterPosition = _tileCharacter.transform.parent.localPosition;
            step.Message = Canvas.DialogContent.text;
            step.UseBackground = Canvas.Background.isActiveAndEnabled;
            step.Highlight = _highlight != null;

            if (indexOf < 0) sequence.Steps.Add(step);

            if (step.Highlight && _highlight != null)
            {
                var identifier = _highlight.gameObject.GetComponent<UIElementIdentifier>();

                if (identifier == null)
                {
                    identifier = _highlight.gameObject.AddComponent<UIElementIdentifier>();
                }

                step.Identifier = identifier.Identifier;
            }

            if (tutorials != null)
            {
                tutorials.GameTutorials = _gameTutorials;
                EditorUtility.SetDirty(tutorials);
            }
        }
#endif

        private void Awake()
        {
            _tileCharacter.gameObject.SetActive(false);
        }
    }
}