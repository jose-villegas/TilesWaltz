using TilesWalk.Tile.Rules;

namespace TilesWalk.Extensions
{
	public static class NeighborWalkRuleExtension
	{
		public static PathBehaviourRule GetPathBehaviour(this NeighborWalkRule source, NeighborWalkRule rule)
		{
			var ruleSet = PathBehaviourRule.None;

			// for when we continue a vertical path
			if ((source == NeighborWalkRule.Down || source == NeighborWalkRule.Up)
			    && rule == NeighborWalkRule.Plain)
			{
				ruleSet |= PathBehaviourRule.VerticalContinuous;
			}

			// for when we continue a horizontal path
			if ((source == NeighborWalkRule.Plain)
			    && rule == NeighborWalkRule.Plain)
			{
				ruleSet |= PathBehaviourRule.HorizontalContinuous;
			}

			// for when we break a horizontal path by going vertical
			if ((source == NeighborWalkRule.Plain)
			    && (rule == NeighborWalkRule.Down || rule == NeighborWalkRule.Up))
			{
				ruleSet |= PathBehaviourRule.HorizontalBreak;
			}

			// for when we break a vertical path by going horizontal
			if ((source == NeighborWalkRule.Down || source == NeighborWalkRule.Up)
			    && (rule == NeighborWalkRule.Down || rule == NeighborWalkRule.Up))
			{
				ruleSet |= PathBehaviourRule.VerticalBreak;
			}

			return ruleSet;
		}
	}
}
