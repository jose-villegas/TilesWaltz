namespace TilesWalk.Tile.Rules
{
	/// <summary>
	/// This class represents the possible rule sets for path building between tiles
	/// </summary>
	public enum PathBehaviourRule
	{
		None = 0,
		VerticalContinuous = 1 << 0,
		HorizontalContinuous = 1 << 1,
		VerticalBreak = 1 << 2,
		HorizontalBreak = 1 << 3,

		// Cases, useful for building logic
		Break = VerticalBreak | HorizontalBreak,
		Continuous = VerticalContinuous | HorizontalContinuous,
		VerticalContinuousOrBreak = VerticalContinuous | Break,
		HorizontalContinuousOrBreak = HorizontalContinuous | Break,
	}
}