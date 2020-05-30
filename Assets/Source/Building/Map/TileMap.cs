using System;
using System.Collections.Generic;
using TilesWalk.BaseInterfaces;
using UnityEngine;

namespace TilesWalk.Building.Map
{
	[Serializable]
	public class TileMap : IModel
	{
		public int Id;
		public List<InsertionInstruction> Instructions;
		public Dictionary<int, Vector3> Tiles;

		public TileMap()
		{
			Id = -1;
			Instructions = new List<InsertionInstruction>();
			Tiles = new Dictionary<int, Vector3>();
		}
	}
}