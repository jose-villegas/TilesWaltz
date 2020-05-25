using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace TilesWalk.General
{
	public class ReactiveTransform
	{
		public Vector3 Position
		{
			get => _position.Value;
			set => _position.Value = value;
		}

		public Quaternion Rotation
		{
			get => _rotation.Value;
			set => _rotation.Value = value;
		}

		public Vector3 Scale
		{
			get => _localScale.Value;
			set => _localScale.Value = value;
		}

		public Vector3 Forward
		{
			get => _forward.Value;
			set => _forward.Value = value;
		}

		public Vector3 Up
		{
			get => _up.Value;
			set => _up.Value = value;
		}

		public Vector3 Right
		{
			get => _right.Value;
			set => _right.Value = value;
		}

		private ReactiveProperty<Vector3> _position;
		private ReactiveProperty<Quaternion> _rotation;
		private ReactiveProperty<Vector3> _localScale;
		private ReactiveProperty<Vector3> _forward;
		private ReactiveProperty<Vector3> _up;
		private ReactiveProperty<Vector3> _right;
		private List<IDisposable> _subcriptions;

		public ReactiveTransform()
		{
			_position = new ReactiveProperty<Vector3>(Vector3.zero);
			_rotation = new ReactiveProperty<Quaternion>(Quaternion.identity);
			_localScale = new ReactiveProperty<Vector3>(Vector3.one);
			_forward = new ReactiveProperty<Vector3>(Vector3.forward);
			_up = new ReactiveProperty<Vector3>(Vector3.up);
			_right = new ReactiveProperty<Vector3>(Vector3.right);
		}

		public void SubscribeTransform(Transform t)
		{
			 _subcriptions.Add(_position.Subscribe(_ => t.position = _));
			 _subcriptions.Add(_rotation.Subscribe(_ => t.rotation = _));
			 _subcriptions.Add(_localScale.Subscribe(_ => t.localScale = _));
			 _subcriptions.Add(t.ObserveEveryValueChanged(t1 => t1.forward).Subscribe(_ => Forward = _));
			_subcriptions.Add(t.ObserveEveryValueChanged(t1 => t1.up).Subscribe(_ => Up = _));
			_subcriptions.Add(t.ObserveEveryValueChanged(t1 => t1.right).Subscribe(_ => Right = _));
		}

		public void Unsubscribe()
		{
			foreach (var subcription in _subcriptions)
			{
				subcription.Dispose();
			}
		}
	}
}