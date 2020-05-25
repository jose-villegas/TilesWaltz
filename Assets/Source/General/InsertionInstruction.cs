using TilesWalk.Tile.Rules;

namespace TilesWalk.General
{
	public class InsertionInstruction
	{
		public int hash;
		public CardinalDirection direction;
		public NeighborWalkRule rule;
	}
}