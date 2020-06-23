using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TilesWalk.Gameplay.Score;

namespace TilesWalk.Gameplay.Persistence
{
	[Serializable]
	public class RecordsKeeper
	{
		[JsonProperty] private Dictionary<string, LevelScore> _records;

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

		public bool Exist(string id, out LevelScore score)
		{
			score = null;
			if (_records == null) return false;

			return _records.TryGetValue(id, out score);
		}

		[JsonIgnore] public int Count => _records.Count;

		[JsonIgnore] public Dictionary<string, LevelScore>.ValueCollection Values => _records.Values;
	}
}