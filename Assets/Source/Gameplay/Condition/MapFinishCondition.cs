using System;
using UniRx;
using UnityEngine;

namespace TilesWalk.Gameplay.Condition
{
	public abstract class MapFinishCondition
	{
		/// <summary>
		/// The map identifier
		/// </summary>
		[SerializeField] protected string _id;

		/// <summary>
		/// The map identifier
		/// </summary>
		public string Id => _id;

		public ReactiveProperty<bool> IsConditionMeet { get; protected set; }
	}

	[Serializable]
	public abstract class MapFinishCondition<T> : MapFinishCondition
	{
		/// <summary>
		/// The class type used for handling the condition
		/// </summary>
		[SerializeField] protected T _limit;

		protected T _tracker;

		public T Limit => _limit;

		public abstract T Update(T value);

		protected MapFinishCondition(string id, T limit)
		{
			_id = id;
			_tracker = default(T);
			_limit = limit;
			IsConditionMeet = new ReactiveProperty<bool>(false);
		}
	}
}