﻿using System;
using UniRx;
using UnityEngine;

namespace TilesWalk.Gameplay.Score
{
	public class Record<T> where T: IComparable<T>
	{
		[SerializeField] private T _highest;
		[SerializeField] private T _lowest;
		[SerializeField] private T _last;

		public T Highest => _highest;
		public T Lowest => _lowest;
		public T Last => _last;

		private Subject<Record<T>> _onRecordUpdated;

		public IObservable<Record<T>> OnRecordUpdatedAsObservable()
		{
			return _onRecordUpdated = _onRecordUpdated ?? new Subject<Record<T>>();
		}

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