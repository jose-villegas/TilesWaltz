using System;
using UnityEngine;

namespace TilesWalk.Gameplay.Score
{
	public class Record<T> where T: IComparable<T>
	{
		[SerializeField] private T _highest;
		[SerializeField] private T _last;

		public T Highest => _highest;
		public T Last => _last;

		public bool Update(T record)
		{
			_last = record;

			if (record.CompareTo(_highest) > 0)
			{
				_highest = record;
				return true;
			}

			return false;
		}

		public Record()
		{
			_highest = default(T);
			_last = default(T);
		}

		public Record(T initial)
		{
			_highest = initial;
			_last = initial;
		}
	}
}