using TilesWalk.General;
using TilesWalk.Tile.Rules;

namespace TilesWalk.Building
{
	/// <summary>
	/// This class describes steps to insert a tile into an
	/// existing tile path
	/// </summary>
	public class InsertionInstruction
	{
		/// <summary>
		/// Identifier for the inserted tile
		/// </summary>
		public int tile;
		/// <summary>
		/// Identifier for the tile where this tile comes from
		/// </summary>
		public int root;

		/// <summary>
		/// The direction this tile must be inserted
		/// </summary>
		public CardinalDirection direction;
		/// <summary>
		/// The walk rule for this tile to be inserted
		/// </summary>
		public NeighborWalkRule rule;
	}
}