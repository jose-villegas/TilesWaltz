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
			// copy neighbor
			tile.Model = _tile.Model;
			tile.InsertionRule = new Tuple<CardinalDirection, NeighborWalkRule>(direction, rule);
			// adjust 3d index according to neighbor
			AdjustNeighborSpace(direction, rule, tile);
			// set 3d actual position to match with hinge points
			tile.Position = tile.Index;
			// connect neighbor references
			_tile.Neighbors[direction] = tile;
			tile.Neighbors[direction.Opposite()] = _tile;
		}

		private void AdjustNeighborSpace(CardinalDirection direction, NeighborWalkRule rule, Tile tile)
		{
			var source = _tile.Index;
			var translate = Vector3.zero;

			switch (direction)
			{
				case CardinalDirection.North:
					translate = TranslateNorth(rule, tile);
					break;
				case CardinalDirection.South:
					translate = TranslateSouth(rule, tile);
					break;
				case CardinalDirection.East:
					translate = TranslateEast(rule, tile);
					break;
				case CardinalDirection.West:
					translate = TranslateWest(rule, tile);
					break;
				default:
					break;
			}

			tile.Index = source + translate;
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

