using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Condition;
using TilesWalk.Gameplay.Score;
using TilesWalk.Map.Tile;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TilesWalk.Map.Bridge
{
	public class LevelBridgePayload
	{
		public LevelMap Level;
		public MapFinishCondition Condition;
		public LevelMapState State;

		/// <summary>
		/// This structure is used to pass level selection information between scenes
		/// </summary>
		/// <param name="level">The map</param>
		/// <param name="condition">The finishing condition</param>
		/// <param name="isFirst">If this is the first time this map is played</param>
		public LevelBridgePayload(LevelMap level, MapFinishCondition condition, LevelMapState state = LevelMapState.None)
		{
			Level = level;
			Condition = condition;
			State = state;
		}
	}

	public class LevelBridge : MonoBehaviour
	{
		public LevelBridgePayload Payload { get; set; } = null;
	}
}