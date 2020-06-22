using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Condition;
using UnityEngine;

namespace TilesWalk.Gameplay.Persistence
{
	[Serializable]
	public class GameMapCollection
	{
		[JsonProperty] [SerializeField] private List<LevelMap> _availableMaps;
		[JsonProperty] [SerializeField] private List<MovesFinishCondition> _movesFinishConditions;
		[JsonProperty] [SerializeField] private List<TimeFinishCondition> _timeFinishConditions;

		[JsonIgnore] public List<LevelMap> AvailableMaps => _availableMaps;
		[JsonIgnore] public List<MovesFinishCondition> MovesFinishConditions => _movesFinishConditions;
		[JsonIgnore] public List<TimeFinishCondition> TimeFinishConditions => _timeFinishConditions;

		public GameMapCollection(List<LevelMap> maps, List<MovesFinishCondition> moves, List<TimeFinishCondition> times)
		{
			_availableMaps = new List<LevelMap>(_availableMaps);
			_movesFinishConditions = new List<MovesFinishCondition>(moves);
			_timeFinishConditions = new List<TimeFinishCondition>(times);
		}

		public GameMapCollection()
		{
		}

		public bool Exist(string id)
		{
			if (AvailableMaps == null || AvailableMaps.Count == 0) return false;

			return AvailableMaps.Exists(x => x.Id == id);
		}

		public void Insert<T>(LevelMap map, T condition) where T : MapFinishCondition
		{
			if (AvailableMaps == null) _availableMaps = new List<LevelMap>();
			if (MovesFinishConditions == null) _movesFinishConditions = new List<MovesFinishCondition>();
			if (TimeFinishConditions == null) _timeFinishConditions = new List<TimeFinishCondition>();

			var indexOf = _availableMaps.FindIndex(x => x.Id == map.Id);

			// this means the level is already save so we are going to replace it instead
			if (indexOf >= 0)
			{
				_availableMaps[indexOf] = map;
			}
			else
			{
				_availableMaps.Add(map);
			}

			// look for condition
			if (map.FinishCondition == FinishCondition.MovesLimit)
			{
				indexOf = _movesFinishConditions.FindIndex(x => x.Id == map.Id);

				if (indexOf >= 0)
				{
					_movesFinishConditions[indexOf] = condition as MovesFinishCondition;
				}
				else
				{
					// check if condition exist on other structure
					indexOf = _timeFinishConditions.FindIndex(x => x.Id == map.Id);

					if (indexOf >= 0) _timeFinishConditions.RemoveAt(indexOf);

					_movesFinishConditions.Add(condition as MovesFinishCondition);
				}
			}

			if (map.FinishCondition == FinishCondition.TimeLimit)
			{
				indexOf = _timeFinishConditions.FindIndex(x => x.Id == map.Id);

				if (indexOf >= 0)
				{
					_timeFinishConditions[indexOf] = condition as TimeFinishCondition;
				}
				else
				{
					// check if condition exist on other structure
					indexOf = _movesFinishConditions.FindIndex(x => x.Id == map.Id);

					if (indexOf >= 0) _movesFinishConditions.RemoveAt(indexOf);

					_timeFinishConditions.Add(condition as TimeFinishCondition);
				}
			}
		}
	}
}