using TilesWalk.General;

namespace TilesWalk.Extensions
{
	public static class CardinalDirectionExtension
	{
		public static CardinalDirection Opposite(this CardinalDirection direction) 
		{
			switch (direction)
			{
				case CardinalDirection.North:
					return CardinalDirection.South;
				case CardinalDirection.South:
					return CardinalDirection.North;
				case CardinalDirection.East:
					return CardinalDirection.West;
				case CardinalDirection.West:
					return CardinalDirection.East;
				default:
					break;
			}

			return CardinalDirection.None;
		}
	}
}

