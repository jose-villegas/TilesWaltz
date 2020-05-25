using System.Collections.Generic;
using UnityEngine;

namespace TilesWalk.Building
{
	public struct TileMap
	{
		public List<InsertionInstruction> instructions;
		public Dictionary<int, Vector3> tiles;
	}
}