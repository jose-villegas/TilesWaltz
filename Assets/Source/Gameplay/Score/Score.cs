using System;
using UnityEngine;

namespace TilesWalk.Gameplay.Score
{
	[Serializable]
	public class Score
	{
		[SerializeField] private int _id;
		[SerializeField] private int _highestScore;
		[SerializeField] private int _lastScore;

		public Score(int id)
		{
			_id = id;
			_highestScore = 0;
			_lastScore = 0;
		}

		public int Id => _id;

		public int HighestScore => _highestScore;

		public int LastScore => _lastScore;

		public void Update(int newScore)
		{
			if (newScore > HighestScore)
			{
				_highestScore = newScore;
			}

			_lastScore = newScore;
		}
	}
}