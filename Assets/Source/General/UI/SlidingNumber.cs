using System;
using System.Collections;
using TilesWalk.Extensions;
using TilesWalk.General.Patterns;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.General.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class SlidingNumber : ObligatoryComponentBehaviour<TextMeshProUGUI>
	{
		[SerializeField] private float _animationSpeed = 2.5f;

		private float _initial;
		private float _target;
		private float _current;
		private bool _isRunning;

		private Subject<float> _onTargetReached;

		public float AnimationSpeed
		{
			get => _animationSpeed;
			set => _animationSpeed = value;
		}

		public float Current
		{
			get => _current;
			set
			{
				_current = value;
				Component.text = ((int)_current).Localize();
			}
		}

		private void OnDestroy()
		{
			_onTargetReached?.OnCompleted();
		}

		public IObservable<float> OnTargetReachedAsObservable()
		{
			return _onTargetReached = _onTargetReached == null ? new Subject<float>() : _onTargetReached;
		}

		public void Target(int value)
		{
			Target((float) value);
		}

		public void Target(float value)
		{
			if (!_isRunning)
			{
				_initial = _current;
				_target = value;
				_isRunning = true;

				StartCoroutine(NumberSlidingAnimation()).GetAwaiter().OnCompleted(() =>
				{
					_initial = value;
					_isRunning = false;
				});
			}
			else
			{
				_target = value;
			}
		}

		private IEnumerator NumberSlidingAnimation()
		{
			while (Math.Abs(Current - _target) > Mathf.Epsilon)
			{
				if (_initial < _target)
				{
					Current += (_animationSpeed * Time.deltaTime) * (_target - _initial);

					if (Current >= _target)
					{
						Current = _target;
					}
				}
				else
				{
					Current -= (_animationSpeed * Time.deltaTime) * (_target - _initial);

					if (Current <= _target)
					{
						Current = _target;
					}
				}

				yield return null;
			}

			Current = _target;
			_onTargetReached?.OnNext(_target);
		}
	}
}