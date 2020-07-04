using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TilesWalk.Gameplay.Score;
using TilesWalk.Map.Tile;

namespace TilesWalk.Gameplay.Persistence
{
	/// <summary>
	/// This class serves as a collection of records matching an Id
	/// with a <see cref="LevelScore"/>
	/// </summary>
	[Serializable]
	public class RecordsKeeper
	{
		[JsonProperty] private Dictionary<string, LevelScore> _records;

		/// <summary>
		/// This will return the matching <see cref="LevelScore"/> for the given
		/// <see cref="id"/>
		/// </summary>
		/// <param name="id">The matching id</param>
		/// <returns></returns>
		public LevelScore this[string id]
		{
			get
			{
				if (_records == null) _records = new Dictionary<string, LevelScore>();

				if (!_records.TryGetValue(id, out var score))
				{
					return _records[id] = new LevelScore(id);
				}

				return score;
			}
		}

		/// <summary>
		/// Determines if such a record exists within the collection
		/// </summary>
		/// <param name="id">The id</param>
		/// <param name="score">The found score</param>
		/// <returns>If the score was found</returns>
		public bool Exist(string id, out LevelScore score)
		{
			score = null;
			if (_records == null) return false;

			return _records.TryGetValue(id, out score);
		}

		/// <summary>
		/// Determines if such a record exists within the collection
		/// </summary>
		/// <param name="id">The id</param>
		/// <returns>If the score was found</returns>
		public bool Exist(string id)
		{
			if (_records == null) return false;

			return _records.ContainsKey(id);
		}

		/// <summary>
		/// A count of how many records are currently stored
		/// </summary>
		[JsonIgnore] public int Count => _records?.Count ?? 0;

		/// <summary>
		/// The collection of records
		/// </summary>
		[JsonIgnore] public Dictionary<string, LevelScore>.ValueCollection Values => _records?.Values;
	}
}