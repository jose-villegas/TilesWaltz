using System.Collections.Generic;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Condition;

namespace TilesWalk.Gameplay.Persistence
{
	public class CustomMaps
	{
		private List<LevelMap> _availableMaps;
		private List<MovesFinishCondition> _movesFinishConditions;
		private List<TimeFinishCondition> _timeFinishConditions;

		public bool Exist(string id)
		{
			if (_availableMaps == null || _availableMaps.Count == 0) return false;

			return _availableMaps.Exists(x => x.Id == id);
		}

		public void Insert<T>(LevelMap map, T condition) where T : MapFinishCondition
		{
			if (_availableMaps == null) _availableMaps = new List<LevelMap>();
			if (_movesFinishConditions == null) _movesFinishConditions = new List<MovesFinishCondition>();
			if (_timeFinishConditions == null) _timeFinishConditions = new List<TimeFinishCondition>();

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