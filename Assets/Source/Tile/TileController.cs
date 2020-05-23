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
			AdjustNeighborSpace(direction, rule, ruleSet, tile);
			// set 3d actual position to match with hinge points
			tile.Position = tile.Index;

			// query translation needed to match points
			var sourceHingePoints = _tile.HingePoints(direction);
			var tileHingePoints = tile.HingePoints(direction.Opposite());
			var move = Vector3.zero;

			//switch (direction)
			//{
			//	case CardinalDirection.North:
			//		if ((ruleSet & PathBehaviourRule.VerticalContinuous) > 0)
			//		{
			//			tile.Position = _tile.Position + Vector3.up;
			//		}
			//		if ((ruleSet & PathBehaviourRule.HorizontalContinuous) > 0)
			//		{
			//			tile.Position = _tile.Position + Vector3.forward;
			//		}
			//		break;
			//	case CardinalDirection.South:
			//		if ((ruleSet & PathBehaviourRule.VerticalContinuous) > 0)
			//		{
			//			tile.Position = _tile.Position + Vector3.down;
			//		}
			//		if ((ruleSet & PathBehaviourRule.HorizontalContinuous) > 0)
			//		{
			//			tile.Position = _tile.Position + Vector3.back;
			//		}
			//		break;
			//	case CardinalDirection.East:
			//		if ((ruleSet & PathBehaviourRule.VerticalContinuous) > 0)
			//		{
			//			tile.Position = _tile.Position + Vector3.forward;
			//		}
			//		if ((ruleSet & PathBehaviourRule.HorizontalContinuous) > 0)
			//		{
			//			tile.Position = _tile.Position + Vector3.right;
			//		}
			//		break;
			//	case CardinalDirection.West:
			//		if ((ruleSet & PathBehaviourRule.VerticalContinuous) > 0)
			//		{
			//			tile.Position = _tile.Position + Vector3.back;
			//		}
			//		if ((ruleSet & PathBehaviourRule.HorizontalContinuous) > 0)
			//		{
			//			tile.Position = _tile.Position + Vector3.left;
			//		}
			//		break;
			//	default:
			//		break;
			//}

			//if ((ruleSet & PathBehaviourRule.Break) > 0)
			//{
			//	switch (direction)
			//	{
			//		case CardinalDirection.North:
			//			tile.Position += sourceHingePoints[2] - tileHingePoints[0];
			//			break;
			//		case CardinalDirection.South:
			//			tile.Position += sourceHingePoints[2] - tileHingePoints[0];
			//			break;
			//		case CardinalDirection.East:
			//			tile.Position += sourceHingePoints[2] - tileHingePoints[0];
			//			break;
			//		case CardinalDirection.West:
			//			tile.Position += sourceHingePoints[2] - tileHingePoints[0];
			//			break;
			//		default:
			//			break;
			//	}
			//}

			// connect neighbor references
			_tile.Neighbors[direction] = tile;
			tile.Neighbors[direction.Opposite()] = _tile;
		}

		private void AdjustNeighborSpace(CardinalDirection direction, NeighborWalkRule rule, PathBehaviourRule ruleSet, Tile tile)
		{
			var source = _tile.Index;
			var translate = Vector3.zero;

			switch (direction)
			{
				case CardinalDirection.North:
					// first take continuity behaviours	
					if ((ruleSet & PathBehaviourRule.HorizontalContinuous) > 0)
					{
						translate = Vector3.forward;
					}
					else if ((ruleSet & PathBehaviourRule.VerticalContinuous) > 0)
					{
						if (_tile.Rule == NeighborWalkRule.Down)
						{
							translate = Vector3.down;
							tile.Rule = NeighborWalkRule.Down;
						}
						else if(_tile.Rule == NeighborWalkRule.Up)
						{
							translate = Vector3.up;
							tile.Rule = NeighborWalkRule.Up;
						}
					}
					// then take on break cases
					if ((ruleSet & PathBehaviourRule.HorizontalBreak) > 0)
					{
						if (rule == NeighborWalkRule.Up)
						{
							translate = Vector3.forward + Vector3.up;
							tile.Rule = NeighborWalkRule.Up;
						}
						else if (rule == NeighborWalkRule.Down)
						{
							translate = Vector3.forward + Vector3.down;
							tile.Rule = NeighborWalkRule.Down;
						}
					}
					else if ((ruleSet & PathBehaviourRule.VerticalBreak) > 0 && _tile.Rule == NeighborWalkRule.Down)
					{
						if (rule == NeighborWalkRule.Up)
						{
							translate = Vector3.forward - Vector3.up;

						}
						else if (rule == NeighborWalkRule.Down)
						{
							translate = Vector3.back - Vector3.up;
						}

						tile.Rule = NeighborWalkRule.Plain;
					}
					else if ((ruleSet & PathBehaviourRule.VerticalBreak) > 0 && _tile.Rule == NeighborWalkRule.Up)
					{
						if (rule == NeighborWalkRule.Up)
						{
							translate = Vector3.back + Vector3.up;

						}
						else if (rule == NeighborWalkRule.Down)
						{
							translate = Vector3.forward + Vector3.up;
						}

						tile.Rule = NeighborWalkRule.Plain;
					}

					break;
				case CardinalDirection.South:
					// first take continuity behaviours	
					if ((ruleSet & PathBehaviourRule.HorizontalContinuous) > 0)
					{
						translate = Vector3.back;
					}
					else if ((ruleSet & PathBehaviourRule.VerticalContinuous) > 0)
					{
						translate = Vector3.down;
					}
					break;
				case CardinalDirection.East:
					if ((ruleSet & PathBehaviourRule.VerticalContinuousOrBreak) > 0)
					{
						translate += Vector3.forward;
					}
					if ((ruleSet & PathBehaviourRule.HorizontalContinuousOrBreak) > 0)
					{
						translate += Vector3.right;
					}
					break;
				case CardinalDirection.West:
					if ((ruleSet & PathBehaviourRule.VerticalContinuousOrBreak) > 0)
					{
						translate += Vector3.back;
					}
					if ((ruleSet & PathBehaviourRule.HorizontalContinuousOrBreak) > 0)
					{
						translate += Vector3.left;
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

