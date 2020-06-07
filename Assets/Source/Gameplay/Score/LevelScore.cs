using System;
using UnityEngine;

namespace TilesWalk.Gameplay.Score
{
	[Serializable]
	public class LevelScore
	{
		[SerializeField] private string _id;
		[SerializeField] private Record<int> _points;
		[SerializeField] private Record<int> _moves;
		[SerializeField] private Record<float> _time;

		public string Id => _id;

		public Record<int> Points => _points;

		public Record<int> Moves => _moves;

		public Record<float> Time => _time;

		public LevelScore(string id)
		{
			_id = id;
			_points = new Record<int>(0);
			_moves = new Record<int>(int.MaxValue);
			_time = new Record<float>(float.MaxValue);
		}
	}
}