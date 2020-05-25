using TilesWalk.General;
using TilesWalk.Tile.Rules;

namespace TilesWalk.Building
{
	public class InsertionInstruction
	{
		public int tile;
		public int root;
		public CardinalDirection direction;
		public NeighborWalkRule rule;
	}
}