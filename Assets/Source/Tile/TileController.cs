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

		public void AddNeighbor(CardinalDirection direction, NeighborWalkRule rule, Tile tile, Transform src,
			Transform isr)
		{
			// copy neighbor
			isr.position = src.position;
			isr.rotation = src.rotation;
			isr.localScale = src.localScale;
			// connect neighbor references
			_tile.Neighbors[direction] = tile;
			tile.Neighbors[direction.Opposite()] = _tile;
			// adjust 3d index according to neighbor
			AdjustNeighborSpace(direction, rule, tile, src, isr);
		}

		private void AdjustNeighborSpace(CardinalDirection direction, NeighborWalkRule rule, Tile tile, Transform src,
			Transform isr)
		{
			tile.Index = _tile.Index;

			Vector3 srcPoint = default;
			Vector3 dstPoint = default;
			Vector3 translate = default;

			switch (direction)
			{
				case CardinalDirection.North:
					translate = TranslateNorth(rule, tile, isr);
					tile.Index += translate;
					CreateHinge(CardinalDirection.North, rule, tile, src, isr, src.forward, isr.forward);
					srcPoint = _tile.HingePoints[CardinalDirection.North];
					dstPoint = tile.HingePoints[CardinalDirection.South];
					break;
				case CardinalDirection.South:
					translate = TranslateSouth(rule, tile, isr);
					tile.Index += translate;
					CreateHinge(CardinalDirection.South, rule, tile, src, isr, -src.forward, -isr.forward);
					srcPoint = _tile.HingePoints[CardinalDirection.South];
					dstPoint = tile.HingePoints[CardinalDirection.North];
					break;
				case CardinalDirection.East:
					translate = TranslateEast(rule, tile, isr);
					tile.Index += translate;
					CreateHinge(CardinalDirection.East, rule, tile, src, isr, src.right, isr.right);
					srcPoint = _tile.HingePoints[CardinalDirection.East];
					dstPoint = tile.HingePoints[CardinalDirection.West];
					break;
				case CardinalDirection.West:
					translate = TranslateWest(rule, tile, isr);
					tile.Index += translate;
					CreateHinge(CardinalDirection.West, rule, tile, src, isr, -src.right, -isr.right);
					srcPoint = _tile.HingePoints[CardinalDirection.West];
					dstPoint = tile.HingePoints[CardinalDirection.East];
					break;
			}

			isr.position = src.position + translate;
			isr.position += srcPoint - dstPoint;
		}

		private void CreateHinge(CardinalDirection direction, NeighborWalkRule rule, Tile tile, Transform src,
			Transform isr,
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
				_tile.HingePoints[direction] -= _tile.Bounds.extents.y * src.up;
				tile.HingePoints[direction.Opposite()] -= tile.Bounds.extents.y * isr.up;
			}
			else if (rule == NeighborWalkRule.Up)
			{
				_tile.HingePoints[direction] += _tile.Bounds.extents.y * src.up;
				tile.HingePoints[direction.Opposite()] += tile.Bounds.extents.y * isr.up;
			}
		}

		private Vector3 TranslateWest(NeighborWalkRule rule, Tile tile, Transform isr)
		{
			// first take continuity behaviour, first the horizontal case:
			var translate = -isr.right;

			if (rule == NeighborWalkRule.Down)
			{
				isr.Rotate(isr.forward, 90, Space.World);
				translate = translate - isr.right;
			}
			else if (rule == NeighborWalkRule.Up)
			{
				isr.Rotate(isr.forward, -90, Space.World);
				translate = translate - isr.right;
			}

			return translate;
		}

		private Vector3 TranslateEast(NeighborWalkRule rule, Tile tile, Transform isr)
		{
			// first take continuity behaviour, first the horizontal case:
			var translate = isr.right;

			if (rule == NeighborWalkRule.Down)
			{
				isr.Rotate(isr.forward, -90, Space.World);
				translate = translate + isr.right;
			}
			else if (rule == NeighborWalkRule.Up)
			{
				isr.Rotate(isr.forward, 90, Space.World);
				translate = translate + isr.right;
			}

			return translate;
		}

		private Vector3 TranslateNorth(NeighborWalkRule rule, Tile tile, Transform isr)
		{
			// first take continuity behaviour, first the horizontal case:
			var translate = isr.forward;

			if (rule == NeighborWalkRule.Down)
			{
				isr.Rotate(isr.right, 90, Space.World);
				translate = translate + isr.forward;
			}
			else if (rule == NeighborWalkRule.Up)
			{
				isr.Rotate(isr.right, -90, Space.World);
				translate = translate + isr.forward;
			}

			return translate;
		}

		private Vector3 TranslateSouth(NeighborWalkRule rule, Tile tile, Transform isr)
		{
			// first take continuity behaviour, first the horizontal case:
			var translate = -isr.forward;

			if (rule == NeighborWalkRule.Down)
			{
				isr.Rotate(isr.right, -90, Space.World);
				translate = translate - isr.forward;
			}
			else if (rule == NeighborWalkRule.Up)
			{
				isr.Rotate(isr.right, 90, Space.World);
				translate = translate - isr.forward;
			}

			return translate;
		}

		internal void AdjustBounds(Bounds bounds)
		{
			_tile.Bounds = bounds;
		}
	}
}