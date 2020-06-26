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

		public float AnimationSpeed
		{
			get => _animationSpeed;
			set => _animationSpeed = value;
		}

		public void Target(int value)
		{
			Target((float) value);
		}

		public void Target(float value)
		{
			if (!_isRunning)
			{
				_current = _initial;
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
			while (Math.Abs(_current - _target) > Mathf.Epsilon)
			{
				if (_initial < _target)
				{
					_current += (_animationSpeed * Time.deltaTime) * (_target - _initial);

					if (_current >= _target)
					{
						_current = _target;
					}
				}
				else
				{
					_current -= (_animationSpeed * Time.deltaTime) * (_target - _initial);

					if (_current <= _target)
					{
						_current = _target;
					}
				}

				Component.text = ((int) _current).Localize();
				yield return null;
			}
		}
	}
}