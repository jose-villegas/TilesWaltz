using System.IO;
using TilesWalk.General;
using TilesWalk.Tile;
using TilesWalk.Tile.Rules;
using UnityEngine;

namespace TilesWalk.Extensions
{
	public static class TileExtension
	{
		public static Vector3[] HingePoints(this Tile.Tile tile, CardinalDirection face)
		{
			var points = new Vector3[8];

			points[0] = tile.Bounds.min;
			points[1] = tile.Bounds.max;
			points[2] = new Vector3(points[0].x, points[0].y, points[1].z);
			points[3] = new Vector3(points[0].x, points[1].y, points[0].z);
			points[4] = new Vector3(points[1].x, points[0].y, points[0].z);
			points[5] = new Vector3(points[0].x, points[1].y, points[1].z);
			points[6] = new Vector3(points[1].x, points[0].y, points[1].z);
			points[7] = new Vector3(points[1].x, points[1].y, points[0].z);

			switch (face)
			{
				case CardinalDirection.North:
					return new Vector3[] { points[1], points[2], points[5], points[6] };
				case CardinalDirection.South:
					return new Vector3[] { points[0], points[3], points[4], points[7] };
				case CardinalDirection.East:
					return new Vector3[] { points[1], points[4], points[6], points[7] };
				case CardinalDirection.West:
					return new Vector3[] { points[0], points[2], points[3], points[5] };
				default:
					break;
			}

			return points;
		}

		public static PathBehaviourRule GetPathBehaviour(NeighborWalkRule source, NeighborWalkRule rule)
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

		public static bool IsValidInsertion(this Tile.Tile source, CardinalDirection direction, NeighborWalkRule rule)
		{
			bool result = false;

			// first check if direction is already occupied
			if (source.Neighbors.ContainsKey(direction))
			{
				return source.Neighbors[direction] == null;
			}
			else
			{
				return true;
			}
			// todo: add rules here
		}
	}
}
