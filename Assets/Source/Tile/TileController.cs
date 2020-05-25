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
		[SerializeField] private Tile _tile;

		public Tile Tile
		{
			get => _tile;
		}

		public TileController()
		{
			_tile = new Tile();
		}

		public void AddNeighbor(CardinalDirection direction, NeighborWalkRule rule, Tile tile)
		{
			// copy neighbor
			tile.Model = _tile.Model;
			tile.InsertionRule = new Tuple<CardinalDirection, NeighborWalkRule>(direction, rule);
			// connect neighbor references
			_tile.Neighbors[direction] = tile;
			tile.Neighbors[direction.Opposite()] = _tile;
			// adjust 3d index according to neighbor
			AdjustNeighborSpace(direction, rule, tile);
		}

		private void AdjustNeighborSpace(CardinalDirection direction, NeighborWalkRule rule, Tile tile)
		{
			tile.Index = _tile.Index;

			Vector3 srcPoint;
			Vector3 dstPoint;
			Vector3 translate;
			switch (direction)
			{
				case CardinalDirection.North:
					translate = TranslateNorth(rule, tile);
					tile.Index += translate;
					CreateHinge(CardinalDirection.North, rule, tile, _tile.Forward, tile.Forward);

					srcPoint = _tile.HingePoints[CardinalDirection.North];
					dstPoint = tile.HingePoints[CardinalDirection.South];
					tile.Position = _tile.Position + translate;
					tile.Position += srcPoint - dstPoint;
					break;
				case CardinalDirection.South:
					translate = TranslateSouth(rule, tile);
					tile.Index += translate;
					CreateHinge(CardinalDirection.South, rule, tile, -_tile.Forward, -tile.Forward);

					srcPoint = _tile.HingePoints[CardinalDirection.South];
					dstPoint = tile.HingePoints[CardinalDirection.North];
					tile.Position = _tile.Position + translate;
					tile.Position += srcPoint - dstPoint;
					break;
				case CardinalDirection.East:
					translate = TranslateEast(rule, tile);
					tile.Index += translate;
					CreateHinge(CardinalDirection.East, rule, tile, _tile.Right, tile.Right);

					srcPoint = _tile.HingePoints[CardinalDirection.East];
					dstPoint = tile.HingePoints[CardinalDirection.West];
					tile.Position = _tile.Position + translate;
					tile.Position += srcPoint - dstPoint;
					break;
				case CardinalDirection.West:
					translate = TranslateWest(rule, tile);
					tile.Index += translate;
					CreateHinge(CardinalDirection.West, rule, tile, -_tile.Right, -tile.Right);

					srcPoint = _tile.HingePoints[CardinalDirection.West];
					dstPoint = tile.HingePoints[CardinalDirection.East];
					tile.Position = _tile.Position + translate;
					tile.Position += srcPoint - dstPoint;
					break;
				default:
					break;
			}
		}

		private void CreateHinge(CardinalDirection direction, NeighborWalkRule rule, Tile tile,
			params Vector3[] forward)
		{
			// parent hinge
			_tile.HingePoints[direction] = _tile.Index;
			_tile.HingePoints[direction] += _tile.Bounds.extents.z * forward[0];
			// inserted hinge
			tile.HingePoints[direction.Opposite()] = tile.Index;
			tile.HingePoints[direction.Opposite()] -= tile.Bounds.extents.z * forward[1];

			if (rule == NeighborWalkRule.Down)
			{
				_tile.HingePoints[direction] -= _tile.Bounds.extents.y * _tile.Up;
				tile.HingePoints[direction.Opposite()] -= tile.Bounds.extents.y * tile.Up;
			}
			else if (rule == NeighborWalkRule.Up)
			{
				_tile.HingePoints[direction] += _tile.Bounds.extents.y * _tile.Up;
				tile.HingePoints[direction.Opposite()] += tile.Bounds.extents.y * tile.Up;
			}
		}

		private Vector3 TranslateWest(NeighborWalkRule rule, Tile tile)
		{
			// first take continuity behaviour, first the horizontal case:
			var translate = -tile.Right;

			if (rule == NeighborWalkRule.Down)
			{
				tile.Rotation = _tile.Rotation * Quaternion.AngleAxis(90, -tile.Forward);
				translate -= TranslateEast(NeighborWalkRule.Plain, tile);
			}
			else if (rule == NeighborWalkRule.Up)
			{
				tile.Rotation = _tile.Rotation * Quaternion.AngleAxis(-90, -tile.Forward);
				translate -= TranslateEast(NeighborWalkRule.Plain, tile);
			}

			return translate;
		}

		private Vector3 TranslateEast(NeighborWalkRule rule, Tile tile)
		{
			// first take continuity behaviour, first the horizontal case:
			var translate = tile.Right;

			if (rule == NeighborWalkRule.Down)
			{
				tile.Rotation = _tile.Rotation * Quaternion.AngleAxis(90, tile.Forward);
				translate += TranslateEast(NeighborWalkRule.Plain, tile);
			}
			else if (rule == NeighborWalkRule.Up)
			{
				tile.Rotation = _tile.Rotation * Quaternion.AngleAxis(-90, tile.Forward);
				translate += TranslateEast(NeighborWalkRule.Plain, tile);
			}

			return translate;
		}

		private Vector3 TranslateNorth(NeighborWalkRule rule, Tile tile)
		{
			// first take continuity behaviour, first the horizontal case:
			var translate = tile.Forward;

			if (rule == NeighborWalkRule.Down)
			{
				tile.Rotation = _tile.Rotation * Quaternion.AngleAxis(-90, tile.Right);
				translate += TranslateNorth(NeighborWalkRule.Plain, tile);
			}
			else if (rule == NeighborWalkRule.Up)
			{
				tile.Rotation = _tile.Rotation * Quaternion.AngleAxis(90, tile.Right);
				translate += TranslateNorth(NeighborWalkRule.Plain, tile);
			}

			return translate;
		}

		private Vector3 TranslateSouth(NeighborWalkRule rule, Tile tile)
		{
			// first take continuity behaviour, first the horizontal case:
			var translate = -tile.Forward;

			if (rule == NeighborWalkRule.Down)
			{
				tile.Rotation = _tile.Rotation * Quaternion.AngleAxis(-90, -tile.Right);
				translate -= TranslateNorth(NeighborWalkRule.Plain, tile);
			}
			else if (rule == NeighborWalkRule.Up)
			{
				tile.Rotation = _tile.Rotation * Quaternion.AngleAxis(90, -tile.Right);
				translate -= TranslateNorth(NeighborWalkRule.Plain, tile);
			}

			return translate;
		}

		internal void AdjustBounds(Bounds bounds)
		{
			_tile.Bounds = bounds;
		}
	}
}