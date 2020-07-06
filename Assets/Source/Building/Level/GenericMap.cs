using System;
using System.Collections.Generic;
using TilesWalk.BaseInterfaces;

namespace TilesWalk.Building.Level
{
	[Serializable]
	public class GenericMap : IModel
	{
		public string Id;
		public List<InsertionInstruction> Instructions;
		public List<RootTile> Roots;
		public int MapSize;

		public GenericMap(GenericMap copyFrom)
		{
			Id = copyFrom.Id;
			Instructions = new List<InsertionInstruction>(copyFrom.Instructions);
			MapSize = copyFrom.MapSize;
		}

		public GenericMap()
		{
			Id = string.Empty;
			Instructions = new List<InsertionInstruction>();
			MapSize = 0;
		}
	}
}