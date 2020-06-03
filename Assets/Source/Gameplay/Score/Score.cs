using System;
using UnityEngine;

namespace TilesWalk.Gameplay.Score
{
	[Serializable]
	public class Score
	{
		[SerializeField] private string _id;
		[SerializeField] private int _highestScore;
		[SerializeField] private int _lastScore;

		public Score(string id)
		{
			_id = id;
			_highestScore = 0;
			_lastScore = 0;
		}

		public string Id => _id;

		public int HighestScore => _highestScore;

		public int LastScore => _lastScore;

		/// <summary>
		/// Updates the current last score
		/// </summary>
		/// <param name="newScore"></param>
		/// <returns>Returns [true] if the highest score was replaced</returns>
		public bool Update(int newScore)
		{
			_lastScore = newScore;

			if (newScore > _highestScore)
			{
				_highestScore = newScore;
				return true;
			}

			return false;
		}

		public static Score operator +(Score a, int b)
		{
			a.Update(a._lastScore + b);
			return a;
		}
	}
}