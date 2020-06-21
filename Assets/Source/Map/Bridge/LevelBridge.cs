using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Condition;
using TilesWalk.Gameplay.Score;
using UnityEngine;

namespace TilesWalk.Map.Bridge
{
	public class LevelBridgePayload
	{
		public LevelMap Level;
		public MapFinishCondition Condition;

		public LevelBridgePayload(LevelMap level, MapFinishCondition condition)
		{
			Level = level;
			Condition = condition;
		}
	}

	public class LevelBridge : MonoBehaviour
	{
		public LevelBridgePayload Payload { get; set; } = null;
	}
}