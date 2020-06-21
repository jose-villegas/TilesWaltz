using System.Collections.Generic;
using TilesWalk.BaseInterfaces;

namespace TilesWalk.Building.Level
{
	public class GenericMap : IModel
	{
		public string Id;
		public List<InsertionInstruction> Instructions;
		public List<int> Tiles;
		public int MapSize;

		public GenericMap(GenericMap copyFrom)
		{
			Id = copyFrom.Id;
			Instructions = new List<InsertionInstruction>(copyFrom.Instructions);
			Tiles = new List<int>(copyFrom.Tiles);
			MapSize = copyFrom.MapSize;
		}

		public GenericMap()
		{
			Id = string.Empty;
			Instructions = new List<InsertionInstruction>();
			Tiles = new List<int>();
			MapSize = 0;
		}
	}
}