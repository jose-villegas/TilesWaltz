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
			// set 3d actual position to match with hinge points
			tile.Position = tile.Index;
		}

		private void AdjustNeighborSpace(CardinalDirection direction, NeighborWalkRule rule, Tile tile)
		{
			tile.Index = _tile.Index;

			switch (direction)
			{
				case CardinalDirection.North:
					tile.Index += TranslateIndex(CardinalDirection.North, rule, tile);
					CreateHinge(CardinalDirection.North, rule, tile, _tile.Forward, tile.Forward);
					break;
				case CardinalDirection.South:
					tile.Index += TranslateIndex(CardinalDirection.South, rule, tile);
					CreateHinge(CardinalDirection.South, rule, tile, -_tile.Forward, -tile.Forward);
					break;
				case CardinalDirection.East:
					tile.Index += TranslateIndex(CardinalDirection.East, rule, tile);
					CreateHinge(CardinalDirection.East, rule, tile, _tile.Right, tile.Right);
					break;
				case CardinalDirection.West:
					tile.Index += TranslateIndex(CardinalDirection.West, rule, tile);
					CreateHinge(CardinalDirection.West, rule, tile, -_tile.Right, -tile.Right);
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

		private Vector3 TranslateIndex(CardinalDirection direction, NeighborWalkRule rule, Tile tile)
		{
			TranslateParameters(direction, tile, out var forward, out var axis, out var sign);
			var translate = forward;

			if (rule == NeighborWalkRule.Down)
			{
				tile.Rotation = _tile.Rotation * Quaternion.AngleAxis(-90 * sign, axis);
				TranslateParameters(direction, tile, out forward, out axis, out sign);
				translate = translate + forward;
			}
			else if (rule == NeighborWalkRule.Up)
			{
				tile.Rotation = _tile.Rotation * Quaternion.AngleAxis(90 * sign, axis);
				TranslateParameters(direction, tile, out forward, out axis, out sign);
				translate = translate + forward;
			}

			return translate;
		}

		private void TranslateParameters(CardinalDirection direction, Tile tile, out Vector3 forward, out Vector3 axis,
			out int sign)
		{
			forward = Vector3.zero;
			axis = Vector3.zero;
			sign = 1;

			switch (direction)
			{
				case CardinalDirection.North:
					forward = tile.Forward;
					axis = tile.Right;
					sign = 1;
					return;
				case CardinalDirection.South:
					forward = -tile.Forward;
					axis = -tile.Right;
					sign = 1;
					return;
				case CardinalDirection.East:
					forward = tile.Right;
					axis = tile.Forward;
					sign = -1;
					return;
				case CardinalDirection.West:
					forward = -tile.Right;
					axis = -tile.Forward;
					sign = 1;
					return;
			}
		}

		internal void AdjustBounds(Bounds bounds)
		{
			_tile.OrientedBounds = bounds;
		}
	}
}