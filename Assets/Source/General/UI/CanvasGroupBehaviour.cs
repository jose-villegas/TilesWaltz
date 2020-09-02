using System;
using System.Collections;
using NaughtyAttributes;
using TilesWalk.Gameplay.Input;
using TilesWalk.General.Patterns;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace TilesWalk.General.UI
{
    /// <summary>
    /// This component extents the functionality for canvas group components,
    /// adding animated Show and Hide transitions and events
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupBehaviour : ObligatoryComponentBehaviour<CanvasGroup>
    {
        [Inject] private GameEventsHandler _gameEvents;

        [Header("Animation")] [SerializeField] private bool _animate;
        [SerializeField] private float _animationTime = 0.1f;
        [SerializeField] private Vector2 _enterDirection = Vector2.up;
        [SerializeField] private Vector2 _exitDirection = Vector2.down;
        [Header("Events")] [SerializeField] private UnityEvent _onCanvasShown;
        [SerializeField] private UnityEvent _onCanvasHidden;

        [Header("Game Events"), SerializeField]
        private bool _triggerGameEvents;

        [SerializeField, ShowIf("_triggerGameEvents"), DisableIf("_showTriggersResume")]
        private bool _showTriggersPause;

        [SerializeField, ShowIf("_triggerGameEvents"), DisableIf("_showTriggersPause")]
        private bool _showTriggersResume;

        [SerializeField, ShowIf("_triggerGameEvents"), DisableIf("_hideTriggersResume")]
        private bool _hideTriggersPause;

        [SerializeField, ShowIf("_triggerGameEvents"), DisableIf("_hideTriggersPause")]
        private bool _hideTriggersResume;

        /// <summary>
        /// Returns if the current alpha value of the <see cref="CanvasGroup"/>
        /// is greater than zero
        /// </summary>
        public bool IsVisible => Component.alpha > 0;

        /// <summary>
        /// Determines if this <see cref="CanvasGroupBehaviour"/> is using
        /// <see cref="Hide"/> and <see cref="Close"/> animations
        /// </summary>
        public bool Animated
        {
            get => _animate;
            set => _animate = value;
        }

        /// <summary>
        /// The direction and magnitude that the canvas follows to appear
        /// </summary>
        public Vector2 EnterDirection
        {
            get => _enterDirection;
            set => _enterDirection = value;
        }

        /// <summary>
        /// The direction and magnitude that the canvas follows to hide
        /// </summary>
        public Vector2 ExitDirection
        {
            get => _exitDirection;
            set => _exitDirection = value;
        }

        /// <summary>
        /// The attached <see cref="RectTransform"/>
        /// </summary>
        private RectTransform Rect
        {
            get
            {
                if (_rect == null)
                {
                    _rect = GetComponent<RectTransform>();
                    _initialAnchor = _rect.anchoredPosition;
                }

                return _rect;
            }
        }

        private Subject<Unit> _onHide;
        private Subject<Unit> _onShow;
        private RectTransform _rect;
        private IEnumerator _animationCoroutine;
        private Vector2 _initialAnchor;

        /// <summary>
        /// Animation for hiding the canvas, follows a direction and magnitude
        /// given by <see cref="ExitDirection"/>
        /// </summary>
        /// <returns></returns>
        private IEnumerator HideAnimation()
        {
            var t = 0f;
            Component.alpha = 1f;

            var src = Rect.anchoredPosition;
            var dst = Rect.anchoredPosition + ExitDirection * 10f;

            while (t <= _animationTime)
            {
                Component.alpha = (1f - t / _animationTime);
                Rect.anchoredPosition = Vector2.Lerp(src, dst, t / _animationTime);
                t += Time.deltaTime;
                yield return null;
            }

            Rect.anchoredPosition = _initialAnchor;

            Component.alpha = 0;
            Component.interactable = false;
            Component.blocksRaycasts = false;

            yield return null;

            _animationCoroutine = null;
            _onCanvasHidden?.Invoke();
            _onHide?.OnNext(new Unit());
        }

        /// <summary>
        /// Animation for showing the canvas, follows a direction and magnitude
        /// given by <see cref="EnterDirection"/>
        /// </summary>
        /// <returns></returns>
        private IEnumerator ShowAnimation()
        {
            var t = 0f;
            Component.alpha = 0f;
            var src = Rect.anchoredPosition + EnterDirection * 10f;
            var dst = Rect.anchoredPosition;
            Rect.anchoredPosition = src;

            while (t <= _animationTime)
            {
                Component.alpha = t / _animationTime;
                Rect.anchoredPosition = Vector2.Lerp(src, dst, t / _animationTime);
                t += Time.deltaTime;
                yield return null;
            }

            Rect.anchoredPosition = _initialAnchor;

            Component.alpha = 1;
            Component.interactable = true;
            Component.blocksRaycasts = true;

            yield return null;

            _animationCoroutine = null;
            _onCanvasShown?.Invoke();
            _onShow?.OnNext(new Unit());
        }

        public virtual void Hide()
        {
            if (_triggerGameEvents)
            {
                if (_hideTriggersPause) _gameEvents.Pause();

                if (_hideTriggersResume) _gameEvents.Resume();
            }

            if (_animate && gameObject.activeInHierarchy)
            {
                if (_animationCoroutine == null)
                {
                    _animationCoroutine = HideAnimation();
                }
                else
                {
                    StopCoroutine(_animationCoroutine);
                    _animationCoroutine = HideAnimation();
                    Rect.anchoredPosition = _initialAnchor;
                }

                StartCoroutine(_animationCoroutine);
            }
            else
            {
                Component.alpha = 0;
                Component.interactable = false;
                Component.blocksRaycasts = false;
                _onCanvasHidden?.Invoke();
                _onHide?.OnNext(new Unit());
            }
        }

        public virtual void Show()
        {
            if (_triggerGameEvents)
            {
                if (_showTriggersPause) _gameEvents.Pause();

                if (_showTriggersResume) _gameEvents.Resume();
            }

            if (_animate && gameObject.activeInHierarchy)
            {
                _animationCoroutine = ShowAnimation();

                if (_animationCoroutine == null)
                {
                    _animationCoroutine = ShowAnimation();
                }
                else
                {
                    StopCoroutine(_animationCoroutine);
                    _animationCoroutine = ShowAnimation();
                    Rect.anchoredPosition = _initialAnchor;
                }

                StartCoroutine(_animationCoroutine);
            }
            else
            {
                Component.alpha = 1;
                Component.interactable = true;
                Component.blocksRaycasts = true;

                _onCanvasShown?.Invoke();
                _onShow?.OnNext(new Unit());
            }
        }

        public void InverseToggle(bool val)
        {
            Toggle(!val);
        }

        public void Toggle(bool val)
        {
            if (val) Show();
            else Hide();
        }

        public IObservable<Unit> OnHideAsObservable()
        {
            return _onHide = _onHide ?? (_onHide = new Subject<Unit>());
        }

        public IObservable<Unit> OnShowAsObservable()
        {
            return _onShow = _onShow ?? (_onShow = new Subject<Unit>());
        }

        protected override void RaiseOnCompletedOnDestroy()
        {
            _onHide?.OnCompleted();
            _onShow?.OnCompleted();
        }
    }
}