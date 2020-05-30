using System.Collections.Generic;
using TilesWalk.BaseInterfaces;
using UnityEngine;

namespace TilesWalk.Building.Map
{
	public class TileMap : IModel
	{
		public List<InsertionInstruction> Instructions;
		public Dictionary<int, Vector3> Tiles;
	}
}