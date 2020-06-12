using System.Collections.Generic;
using TilesWalk.BaseInterfaces;

namespace TilesWalk.Building.Level
{
	public class GenericMap : IModel
	{
		public string Id;
		public List<InsertionInstruction> Instructions;
		public List<int> Tiles;
	}
}