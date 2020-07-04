using System;
using Newtonsoft.Json;
using UniRx;
using UnityEngine;

namespace TilesWalk.Gameplay.Score
{
	/// <summary>
	/// Class used for recording data, this is useful for keeping
	/// record of points, or other player statistics
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Record<T> where T: IComparable<T>
	{
		[JsonProperty] [SerializeField] private T _highest;
		[JsonProperty] [SerializeField] private T _lowest;
		[JsonProperty] [SerializeField] private T _last;

		[JsonIgnore] public T Highest => _highest;
		[JsonIgnore] public T Lowest => _lowest;
		[JsonIgnore] public T Last => _last;

		private Subject<Record<T>> _onRecordUpdated;

		/// <summary>
		/// Called after a record values are updated, <see cref="Update"/>
		/// </summary>
		/// <returns></returns>
		public IObservable<Record<T>> OnRecordUpdatedAsObservable()
		{
			return _onRecordUpdated = _onRecordUpdated ?? new Subject<Record<T>>();
		}

		/// <summary>
		/// Update the record last value. <see cref="Highest"/> and <see cref="Lowest"/>
		/// will be updated here too if matching the conditions
		/// </summary>
		/// <param name="record"></param>
		public void Update(T record)
		{
			_last = record;

			if (record.CompareTo(_highest) > 0)
			{
				_highest = record;
			}

			if (record.CompareTo(_lowest) < 0)
			{
				_lowest = record;
			}

			_onRecordUpdated?.OnNext(this);
		}

		public Record()
		{
			_highest = default(T);
			_last = default(T);
		}

		public Record(T initial)
		{
			_lowest = initial;
			_highest = initial;
			_last = initial;
		}
	}
}