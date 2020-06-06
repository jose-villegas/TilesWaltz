using System;
using UnityEngine;

namespace TilesWalk.Gameplay.Score
{
	[Serializable]
	public class LevelScore
	{
		[SerializeField] private string _id;
		[SerializeField] private Record<int> _points;

		public string Id => _id;

		public Record<int> Points => _points;

		public LevelScore(string id)
		{
			_id = id;
			_points = new Record<int>(0);
		}
	}
}