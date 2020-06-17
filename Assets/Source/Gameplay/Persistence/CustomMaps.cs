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
	}
}