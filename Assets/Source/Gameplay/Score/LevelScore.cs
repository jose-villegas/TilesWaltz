using System;
using Newtonsoft.Json;
using UnityEngine;

namespace TilesWalk.Gameplay.Score
{
	[Serializable]
	public class LevelScore
	{
		[JsonProperty] [SerializeField] private string _id;
		[JsonProperty] [SerializeField] private Record<int> _points;
		[JsonProperty] [SerializeField] private Record<int> _moves;
		[JsonProperty] [SerializeField] private Record<float> _time;

		[JsonIgnore] public string Id => _id;

		[JsonIgnore] public Record<int> Points => _points;

		[JsonIgnore] public Record<int> Moves => _moves;

		[JsonIgnore] public Record<float> Time => _time;

		public LevelScore(string id)
		{
			_id = id;
			_points = new Record<int>(0);
			_moves = new Record<int>(int.MaxValue);
			_time = new Record<float>(float.MaxValue);
		}
	}
}