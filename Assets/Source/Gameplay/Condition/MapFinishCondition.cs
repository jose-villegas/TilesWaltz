using System;
using UniRx;
using UnityEngine;

namespace TilesWalk.Gameplay.Condition
{
	[Serializable]
	public abstract class MapFinishCondition<T>
	{
		/// <summary>
		/// The map identifier
		/// </summary>
		[SerializeField] protected string _id;
		/// <summary>
		/// The class type used for handling the condition
		/// </summary>
		[SerializeField] protected T _handler;

		public ReactiveProperty<bool> IsConditionMeet { get; protected set; }

		/// <summary>
		/// The map identifier
		/// </summary>
		protected string Id => _id;

		/// <summary>
		/// The class type used for handling the condition
		/// </summary>
		protected T Handler => _handler;

		protected abstract T UpdateHandler();

		protected MapFinishCondition(string id, T initial)
		{
			_handler = initial;
			IsConditionMeet = new ReactiveProperty<bool>(false);
		}
	}
}