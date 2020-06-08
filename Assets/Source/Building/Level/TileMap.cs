using System;
using System.Collections.Generic;
using TilesWalk.BaseInterfaces;
using TilesWalk.Gameplay.Condition;

namespace TilesWalk.Building.Level
{
	[Serializable]
	public class TileMap : IModel
	{
		public string Id;
		public int Target;
		public List<InsertionInstruction> Instructions;
		public List<int> Tiles;
		public FinishCondition FinishCondition;

		/// <summary>
		/// Useful for map building
		/// </summary>
		public List<string> TileParameters;

		public TileMap()
		{
			Id = "-1";
			Instructions = new List<InsertionInstruction>();
			TileParameters = new List<string>();
			Tiles = new List<int>();
		}
	}
}