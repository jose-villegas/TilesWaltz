using System;
using NaughtyAttributes;
using TilesWalk.BaseInterfaces;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace TilesWalk.General.Patterns
{
	public abstract class GenericFactory<T> : ObservableTriggerBase, IFactory<T>
	{
		[SerializeField] protected GameObject Asset;

		private Subject<T> _onNewInstance;

		[Button]
		public T NewInstance()
		{
			var instance= CreateInstance();
			_onNewInstance?.OnNext(instance);
			return instance;
		}

		protected abstract T CreateInstance();

		protected override void RaiseOnCompletedOnDestroy()
		{
			_onNewInstance?.OnCompleted();
		}

		public IObservable<T> OnNewInstanceAsObservable()
		{
			return _onNewInstance = _onNewInstance ?? new Subject<T>();
		}
	}
}