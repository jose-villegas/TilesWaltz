using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Score;
using UnityEngine;

namespace TilesWalk.Navigation.Map
{
	public class MapLevelBridge : MonoBehaviour
	{
		public TileMap SelectedLevel { get; set; } = null;
		public LevelScore Results { get; set; } = null;
	}
}