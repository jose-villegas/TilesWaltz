namespace TilesWalk.General
{
	public enum CardinalDirection
	{
		North = 1 << 0,
		South = 1 << 1,
		East = 1 << 2,
		West = 1 << 3,

		Sides = East | West,
		Axis = North | South,
	}
}
