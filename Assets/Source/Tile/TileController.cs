using System;
using System.IO;
using TilesWalk.BaseInterfaces;
using TilesWalk.Extensions;
using TilesWalk.General;
using TilesWalk.Tile.Rules;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace TilesWalk.Tile
{
	[Serializable]
	public class TileController : IController
	{
		[SerializeField]
		private Tile _tile;

		public Tile Tile { get => _tile; }

		public TileController()
		{
			_tile = new Tile();
		}

		public void AddNeighbor(CardinalDirection direction, NeighborWalkRule rule, Tile tile)
		{
			// obtain neighbouring behavior
			var ruleSet = _tile.GetPathBehaviour(rule);
			// adjust 3d index according to neighbor
			AdjustNeighborIndex(direction, ruleSet, tile);

			// set orientation 
			tile.Orientation = TileExtension.Orientation(rule);

			// set 3d actual position to match with hinge points
			tile.Position = tile.Index;

			// query translation needed to match points
			var sourceHingePoints = _tile.HingePoints(direction);
			var tileHingePoints = tile.HingePoints(direction.Opposite());
			var move = Vector3.zero;

			switch (direction)
			{
				case CardinalDirection.North:
					if ((ruleSet & PathBehaviourRule.VerticalContinuous) > 0)
					{
						tile.Position = _tile.Position + Vector3.up;
					}
					if ((ruleSet & PathBehaviourRule.HorizontalContinuous) > 0)
					{
						tile.Position = _tile.Position + Vector3.forward;
					}
					break;
				case CardinalDirection.South:
					if ((ruleSet & PathBehaviourRule.VerticalContinuous) > 0)
					{
						tile.Position = _tile.Position + Vector3.down;
					}
					if ((ruleSet & PathBehaviourRule.HorizontalContinuous) > 0)
					{
						tile.Position = _tile.Position + Vector3.back;
					}
					break;
				case CardinalDirection.East:
					if ((ruleSet & PathBehaviourRule.VerticalContinuous) > 0)
					{
						tile.Position = _tile.Position + Vector3.forward;
					}
					if ((ruleSet & PathBehaviourRule.HorizontalContinuous) > 0)
					{
						tile.Position = _tile.Position + Vector3.right;
					}
					break;
				case CardinalDirection.West:
					if ((ruleSet & PathBehaviourRule.VerticalContinuous) > 0)
					{
						tile.Position = _tile.Position + Vector3.back;
					}
					if ((ruleSet & PathBehaviourRule.HorizontalContinuous) > 0)
					{
						tile.Position = _tile.Position + Vector3.left;
					}
					break;
				default:
					break;
			}

			if ((ruleSet & PathBehaviourRule.Break) > 0)
			{
				switch (direction)
				{
					case CardinalDirection.North:
						tile.Position += sourceHingePoints[2] - tileHingePoints[0];
						break;
					case CardinalDirection.South:
						tile.Position += sourceHingePoints[2] - tileHingePoints[0];
						break;
					case CardinalDirection.East:
						tile.Position += sourceHingePoints[2] - tileHingePoints[0];
						break;
					case CardinalDirection.West:
						tile.Position += sourceHingePoints[2] - tileHingePoints[0];
						break;
					default:
						break;
				}
			}

			// connect neighbor references
			_tile.Neighbors[direction] = tile;
			tile.Neighbors[direction.Opposite()] = _tile;
		}

		private void AdjustNeighborIndex(CardinalDirection direction, PathBehaviourRule ruleSet, Tile tile)
		{
			var source = _tile.Index;
			var translate = Vector3Int.zero;

			switch (direction)
			{
				case CardinalDirection.North:
					if ((ruleSet & PathBehaviourRule.VerticalContinuousOrBreak) > 0)
					{
						translate += Vector3Int.up;
					}
					if ((ruleSet & PathBehaviourRule.HorizontalContinuousOrBreak) > 0)
					{
						translate += Vector3IntExtension.forward();
					}
					break;
				case CardinalDirection.South:
					if ((ruleSet & PathBehaviourRule.VerticalContinuousOrBreak) > 0)
					{
						translate += Vector3Int.down;
					}
					if ((ruleSet & PathBehaviourRule.HorizontalContinuousOrBreak) > 0)
					{
						translate += Vector3IntExtension.backward();
					}
					break;
				case CardinalDirection.East:
					if ((ruleSet & PathBehaviourRule.VerticalContinuousOrBreak) > 0)
					{
						translate += Vector3IntExtension.forward();
					}
					if ((ruleSet & PathBehaviourRule.HorizontalContinuousOrBreak) > 0)
					{
						translate += Vector3Int.right;
					}
					break;
				case CardinalDirection.West:
					if ((ruleSet & PathBehaviourRule.VerticalContinuousOrBreak) > 0)
					{
						translate += Vector3IntExtension.backward();
					}
					if ((ruleSet & PathBehaviourRule.HorizontalContinuousOrBreak) > 0)
					{
						translate += Vector3Int.left;
					}
					break;
				default:
					break;
			}

			tile.Index = source + translate;
		}

		internal void AdjustBounds(Bounds bounds)
		{
			_tile.Bounds = bounds;
		}
	}
}

