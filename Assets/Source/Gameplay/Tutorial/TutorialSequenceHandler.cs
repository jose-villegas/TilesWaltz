using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIExtensions;
using NaughtyAttributes;
using TilesWalk.Gameplay.Animation;
using TilesWalk.Gameplay.Tutorial.UI;
using TilesWalk.General.UI;
using TilesWalk.Gameplay.Tutorial.Installer;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Zenject;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace TilesWalk.Gameplay.Tutorial
{
    /// <summary>
    /// This class handles the visualization in-game of a tutorial sequence
    /// </summary>
    public class TutorialSequenceHandler : ObservableTriggerBase
    {
        [Inject(Id = "GameTutorials"), SerializeField]
        private List<TutorialSequence> _gameTutorials;

        [Inject] private AnimationConfiguration _animation;

        /// <summary>
        /// The tile talking character
        /// </summary>
        [SerializeField] private TutorialTileCharacter _tileCharacter;

        private Subject<TutorialStep> _onStepSetupCompleted;
        private Subject<TutorialTileCharacter> _onCharacterMovementCompleted;
        private TutorialSequence _currentSequence;
        private TutorialCanvas _canvas;
        private int _currentStepIndex;
        private Coroutine _runningCoroutine;
        private List<GameObject> _copiedElement = new List<GameObject>();
        private Canvas _overlayCanvas;
        private List<string> _currentPhrases;
        private IDisposable _clickDispose;


        public TutorialCanvas Canvas
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

        public Canvas OverlayCanvas
        {
            get
            {
                if (_overlayCanvas == null)
                {
                    _overlayCanvas = GetComponentInParent<Canvas>();
                }

                return _overlayCanvas;
            }
        }

        /// <summary>
        /// The tile talking character
        /// </summary>
        public TutorialTileCharacter TileCharacter => _tileCharacter;

        public TutorialSequence CurrentSequence => _currentSequence;
        public int CurrentStepIndex => _currentStepIndex;

#if UNITY_EDITOR
        [Button(enabledMode: EButtonEnableMode.Editor)]
        private void LoadTutorials()
        {
            if (_gameTutorials == null || _gameTutorials.Count == 0)
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
            step.Message = Canvas.DialogContent.Component.text;
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

        public IObservable<TutorialStep> OnStepSetupCompletedAsObservable()
        {
            return _onStepSetupCompleted == null
                ? _onStepSetupCompleted = new Subject<TutorialStep>()
                : _onStepSetupCompleted;
        }

        public IObservable<TutorialTileCharacter> OnCharacterMovementCompletedAsObservable()
        {
            return _onCharacterMovementCompleted == null
                ? _onCharacterMovementCompleted = new Subject<TutorialTileCharacter>()
                : _onCharacterMovementCompleted;
        }

        protected override void RaiseOnCompletedOnDestroy()
        {
            _onStepSetupCompleted?.OnCompleted();
            _onCharacterMovementCompleted?.OnCompleted();
        }

        /// <summary>
        /// This moves the <see cref="TileCharacter"/> to another position
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private IEnumerator MoveCharacter(Vector3 target)
        {
            var source = _tileCharacter.transform.parent.localPosition;
            var t = 0f;

            while (t < _animation.CharacterMovementTime)
            {
                _tileCharacter.transform.parent.localPosition = Vector3.Lerp(source, target, t);
                t += Time.deltaTime / _animation.CharacterMovementTime;
                yield return null;
            }

            _tileCharacter.transform.parent.localPosition = target;
            _runningCoroutine = null;
            _onCharacterMovementCompleted?.OnNext(_tileCharacter);
        }

        /// <summary>
        /// This prepares the sequence handler to manage the given sequence id.
        /// The sequence handle can only manage one tutorial sequence at a time.
        /// Use <see cref="NextStep"/> to start the sequence.
        /// </summary>
        /// <param name="sequenceId"></param>
        /// <returns></returns>
        public TutorialSequenceHandler SetupForSequence(string sequenceId)
        {
            // first find if this sequence actually exists
            var indexOf = _gameTutorials.FindIndex(x => x.SequenceId == sequenceId);

            if (indexOf >= 0)
            {
                _currentSequence = _gameTutorials[indexOf];
                _currentStepIndex = 0;
                // todo: check for conflict, when two sequences want to play
                _clickDispose?.Dispose();
            }
            else
            {
                return null;
            }

            return this;
        }

        /// <summary>
        /// From the current sequence, moves to the next step, if the sequence just happens
        /// to be recently setup the next step would be the first.
        /// </summary>
        /// <returns></returns>
        public bool NextStep()
        {
            if (_currentStepIndex >= 0 && _currentStepIndex < _currentSequence.Steps.Count)
            {
                // if we are in a intermediate step we need to clear the previous data
                if (_currentStepIndex > 0)
                {
                    DiscardPreviousStep();
                }

                var step = _currentSequence.Steps[_currentStepIndex];

                // setup highlight
                if (step.Highlight)
                {
                    // find ui element
                    if (!UIElementIdentifier.Registered.TryGetValue(step.Identifier, out var identifier))
                    {
                        Debug.LogError($"Couldn't find the UIElement with the given id {step.Identifier}");
                    }
                    else
                    {
                        HandleHighlight(step, identifier);
                    }

                    // handles multiple highlights
                    if (step.Identifiers != null && step.Identifiers.Count > 0)
                    {
                        foreach (var stepIdentifier in step.Identifiers)
                        {
                            // find ui element
                            if (!UIElementIdentifier.Registered.TryGetValue(stepIdentifier, out identifier))
                            {
                                Debug.LogError($"Couldn't find the UIElement with the given id {step.Identifier}");
                            }
                            else
                            {
                                HandleHighlight(step, identifier);
                            }
                        }
                    }
                }

                if (_runningCoroutine != null)
                {
                    StopCoroutine(_runningCoroutine);
                }

                // start moving
                _runningCoroutine = StartCoroutine(MoveCharacter(step.CharacterPosition));
                // show dialog
                Canvas.DialogContent.ChangeText(step.Message);
                // determine if we need the background
                Canvas.Background.gameObject.SetActive(step.UseBackground);
                // finally show the elements
                Canvas.Show();
                _tileCharacter.gameObject.SetActive(true);

                _onStepSetupCompleted?.OnNext(step);
                _currentStepIndex++;
                return true;
            }

            return false;
        }

        private void HandleHighlight(TutorialStep step, UIElementIdentifier identifier)
        {
            // bring to front through sorting
            if (step.BringToFront)
            {
                // first copy the object on our canvas
                var instance = Instantiate(identifier.gameObject, Canvas.Background.transform);
                instance.transform.localPosition = identifier.transform.localPosition;
                var rectTransform = instance.GetComponent<RectTransform>();
                var srcRectTransform = identifier.GetComponent<RectTransform>();
                rectTransform.sizeDelta = srcRectTransform.sizeDelta;

                instance.layer = gameObject.layer;

                // first add highlight, first the first graphic component
                var graphic = instance.gameObject.GetComponentInChildren<Graphic>();
                // add shadow
                var shadow = graphic.gameObject.AddComponent<UIShadow>();

                shadow.style = ShadowStyle.Outline;
                shadow.effectColor = Color.cyan;
                shadow.effectDistance = Vector2.one * 2.5f;

                var canvas = instance.gameObject.AddComponent<Canvas>();
                canvas.overrideSorting = true;
                canvas.sortingLayerID = OverlayCanvas.sortingLayerID;
                canvas.sortingOrder = 999;

                if (step.Interactable)
                {
                    var raycaster = instance.gameObject.AddComponent<GraphicRaycaster>();
                }

                _copiedElement.Add(instance);
            }
            else
            {
                // first add highlight, first the first graphic component
                var graphic = identifier.gameObject.GetComponentInChildren<Graphic>();
                // add shadow
                var shadow = graphic.gameObject.AddComponent<UIShadow>();

                shadow.style = ShadowStyle.Outline;
                shadow.effectColor = Color.cyan;
                shadow.effectDistance = Vector2.one * 2.5f;
            }
        }

        /// <summary>
        /// This handles getting rid of the changes made by the previous step
        /// </summary>
        private void DiscardPreviousStep()
        {
            var previousIndex = _currentStepIndex - 1;
            var prevStep = _currentSequence.Steps[previousIndex];

            // setup highlight
            if (prevStep.Highlight)
            {
                if (_copiedElement.Count > 0)
                {
                    foreach (var o in _copiedElement)
                    {
                        Destroy(o);
                    }

                    _copiedElement = new List<GameObject>();
                }

                // find ui element
                if (UIElementIdentifier.Registered.TryGetValue(prevStep.Identifier, out var identifier))
                {
                    if (!prevStep.BringToFront)
                    {
                        var shadow = identifier.GetComponentInChildren<UIShadow>();
                        shadow.enabled = false;
                    }
                }
                else
                {
                    Debug.LogWarning($"Couldn't find the UIElement with the given id {prevStep.Identifier}");
                }

                if (prevStep.Identifiers != null && prevStep.Identifiers.Count > 0)
                {
                    foreach (var stepIdentifier in prevStep.Identifiers)
                    {
                        // find ui element
                        if (UIElementIdentifier.Registered.TryGetValue(stepIdentifier, out identifier))
                        {
                            if (!prevStep.BringToFront)
                            {
                                var shadow = identifier.GetComponentInChildren<UIShadow>();
                                shadow.enabled = false;
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"Couldn't find the UIElement with the given id {prevStep.Identifier}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Once a sequence is finished this method must be called.
        /// It handles clean up of the current sequence.
        /// Some sequences may leave the character around with random phrases, this
        /// method handles that behavior.
        /// </summary>
        public void FinishSequence()
        {
            DiscardPreviousStep();

            if (_currentSequence.HideCharacterAfter)
            {
                _tileCharacter.gameObject.SetActive(false);
            }
            else
            {
                if (_currentSequence.PossiblePhrases != null && _currentSequence.PossiblePhrases.Count > 0)
                {
                    Canvas.Background.gameObject.SetActive(false);
                    _currentPhrases = new List<string>(_currentSequence.PossiblePhrases);

                    if (_runningCoroutine != null)
                    {
                        StopCoroutine(_runningCoroutine);
                    }

                    _runningCoroutine = StartCoroutine(MoveCharacter(_currentSequence.LastPosition));

                    _clickDispose = _tileCharacter.OnTileCharacterClickedAsObservable().Subscribe(character =>
                    {
                        Canvas.DialogContent.ChangeText(_currentPhrases[Random.Range(0, _currentPhrases.Count)]);
                        Canvas.Show();
                        Canvas.DialogContent.OnTextDialogCompletedAsObservable()
                            .Delay(TimeSpan.FromSeconds(2f))
                            .Subscribe(done => { Canvas.Hide(); });
                    }).AddTo(this);
                }
            }

            _currentSequence = null;
            _currentStepIndex = 0;
        }
    }
}