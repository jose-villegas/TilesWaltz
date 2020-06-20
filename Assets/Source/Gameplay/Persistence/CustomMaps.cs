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

		public List<LevelMap> AvailableMaps => _availableMaps;
		public List<MovesFinishCondition> MovesFinishConditions => _movesFinishConditions;
		public List<TimeFinishCondition> TimeFinishConditions => _timeFinishConditions;

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

			var indexOf = AvailableMaps.FindIndex(x => x.Id == map.Id);

			// this means the level is already save so we are going to replace it instead
			if (indexOf >= 0)
			{
				AvailableMaps[indexOf] = map;
			}
			else
			{
				AvailableMaps.Add(map);
			}

			// look for condition
			if (map.FinishCondition == FinishCondition.MovesLimit)
			{
				indexOf = MovesFinishConditions.FindIndex(x => x.Id == map.Id);

				if (indexOf >= 0)
				{
					MovesFinishConditions[indexOf] = condition as MovesFinishCondition;
				}
				else
				{
					// check if condition exist on other structure
					indexOf = TimeFinishConditions.FindIndex(x => x.Id == map.Id);

					if (indexOf >= 0) TimeFinishConditions.RemoveAt(indexOf);

					MovesFinishConditions.Add(condition as MovesFinishCondition);
				}
			}

			if (map.FinishCondition == FinishCondition.TimeLimit)
			{
				indexOf = TimeFinishConditions.FindIndex(x => x.Id == map.Id);

				if (indexOf >= 0)
				{
					TimeFinishConditions[indexOf] = condition as TimeFinishCondition;
				}
				else
				{
					// check if condition exist on other structure
					indexOf = MovesFinishConditions.FindIndex(x => x.Id == map.Id);

					if (indexOf >= 0) MovesFinishConditions.RemoveAt(indexOf);

					TimeFinishConditions.Add(condition as TimeFinishCondition);
				}
			}
		}
	}
}