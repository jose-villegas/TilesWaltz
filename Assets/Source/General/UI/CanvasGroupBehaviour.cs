using System;
using System.Collections;
using System.Collections.Generic;
using TilesWalk.General.Patterns;
using TilesWalk.Navigation.UI;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TilesWalk.General.UI
{
	[RequireComponent(typeof(CanvasGroup))]
	public class CanvasGroupBehaviour : ObligatoryComponentBehaviour<CanvasGroup>
	{
		[Header("Animation")]
		[SerializeField] private bool _animate;
		[SerializeField] private float _animationTime = 0.1f;
		[Header("Events")]
		[SerializeField] private UnityEvent _onCanvasShown;
		[SerializeField] private UnityEvent _onCanvasHidden;

		public bool IsVisible => Component.alpha > 0;

		public Vector2 EnterDirection { get; set; } = Vector2.up;

		public Vector2 ExitDirection { get; set; } = Vector2.down;

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
		private IEnumerator _showCoroutine;
		private IEnumerator _hideCouroutine;
		private Vector2 _initialAnchor;

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

			_onCanvasHidden?.Invoke();
			_onHide?.OnNext(new Unit());
			_hideCouroutine = null;
		}

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

			_onCanvasShown?.Invoke();
			_onShow?.OnNext(new Unit());
			_showCoroutine = null;
		}

		public virtual void Hide()
		{
			if (_animate)
			{
				if (_hideCouroutine == null)
				{
					_hideCouroutine = HideAnimation();
				}
				else
				{
					StopCoroutine(_hideCouroutine);
					_hideCouroutine = HideAnimation();
					Rect.anchoredPosition = _initialAnchor;
				}

				StartCoroutine(_hideCouroutine);
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
			if (_animate)
			{
				_showCoroutine = ShowAnimation();

				if (_showCoroutine == null)
				{
					_showCoroutine = ShowAnimation();
				}
				else
				{
					StopCoroutine(_showCoroutine);
					_showCoroutine = ShowAnimation();
					Rect.anchoredPosition = _initialAnchor;
				}

				StartCoroutine(_showCoroutine);
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