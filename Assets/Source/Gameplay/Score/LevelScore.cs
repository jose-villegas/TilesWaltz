using System;
using Newtonsoft.Json;
using TilesWalk.Building.Level;
using UnityEngine;

namespace TilesWalk.Gameplay.Score
{
	/// <summary>
	/// This class score information about a <see cref="LevelMap"/>
	/// </summary>
	[Serializable]
	public class LevelScore
	{
		/// <summary>
		/// This id represents the matching key for the <see cref="GenericMap.Id"/>
		/// </summary>
		[JsonProperty] [SerializeField] private string _id;
		/// <summary>
		/// The level points summary
		/// </summary>
		[JsonProperty] [SerializeField] private Record<int> _points;
		/// <summary>
		/// The moves records
		/// </summary>
		[JsonProperty] [SerializeField] private Record<int> _moves;
		/// <summary>
		/// The time records
		/// </summary>
		[JsonProperty] [SerializeField] private Record<float> _time;

		/// <summary>
		/// This id represents the matching key for the <see cref="GenericMap.Id"/>
		/// </summary>
		[JsonIgnore] public string Id => _id;


		/// <summary>
		/// The level points summary
		/// </summary>
		[JsonIgnore] public Record<int> Points => _points;

		/// <summary>
		/// The moves records
		/// </summary>
		[JsonIgnore] public Record<int> Moves => _moves;

		/// <summary>
		/// This id represents the matching key for the <see cref="GenericMap.Id"/>
		/// </summary>
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