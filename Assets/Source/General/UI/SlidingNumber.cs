using System;
using System.Collections;
using TilesWalk.Extensions;
using TilesWalk.General.Patterns;
using TMPro;
using UniRx;
using UnityEngine;

namespace TilesWalk.General.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class SlidingNumber : ObligatoryComponentBehaviour<TextMeshProUGUI>
	{
		[SerializeField] private float _animationSpeed = 5f;

		private float _initial;
		private float _target;
		private float _current;
		private bool _isRunning;

		private void Awake()
		{
			Component.text = 0.Localize();
			Component.ObserveEveryValueChanged(x => x.text).Subscribe(OnValueChanged).AddTo(this);
		}

		private void OnValueChanged(string value)
		{
			if (!_isRunning)
			{
				Component.text = ((int)_initial).Localize(); ;
				_current = _initial;
				_target = float.Parse(value);
				_isRunning = true;

				StartCoroutine(NumberSlidingAnimation()).GetAwaiter().OnCompleted(() =>
				{
					_initial = float.Parse(value);
					_isRunning = false;
				});
			}
			else
			{
				if (Math.Abs(_current - _target) > Mathf.Epsilon) return;

				_target = float.Parse(value);
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
