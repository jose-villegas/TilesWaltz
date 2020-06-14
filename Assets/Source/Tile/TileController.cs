using System;
using System.Collections.Generic;
using System.Linq;
using TilesWalk.BaseInterfaces;
using TilesWalk.Extensions;
using TilesWalk.General;
using TilesWalk.Tile.Rules;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace TilesWalk.Tile
{
	[Serializable]
	public partial class TileController : IController
	{
		[SerializeField] private Tile _tile;

		public Tile Tile => _tile;

		public TileController()
		{
			_tile = new Tile();
		}

		public TileController(Tile tile)
		{
			_tile = tile;
		}

		/// <summary>
		/// This methods inserts a neighbor in the data structure of the tile path
		/// </summary>
		/// <param name="direction">The direction respective to this tile as root</param>
		/// <param name="rule">The insertion rule for this tile</param>
		/// <param name="tile">The tile data, to be referenced</param>
		public void AddNeighbor(CardinalDirection direction, NeighborWalkRule rule, Tile tile, Matrix4x4 root,
			out Vector3 translate, out Quaternion rotate)
		{
			// connect neighbor references
			_tile.Neighbors[direction] = tile;
			tile.Neighbors[direction.Opposite()] = _tile;
			tile.ShuffleColor();
			// adjust 3d index according to neighbor
			AdjustNeighborSpace(direction, rule, tile, root, out translate, out rotate);
			// refresh shortest path for all related neighbors
			ChainRefreshPaths(tile);
		}

		public static void ChainRefreshPaths(Tile source, CardinalDirection ignore = CardinalDirection.None,
			bool updateColorPath = true, bool updateShortestPath = true)
		{
			if (updateColorPath) source.RefreshMatchingColorPatch();
			if (updateShortestPath) source.RefreshShortestLeafPath();

			foreach (var neighbor in source.Neighbors)
			{
				if (neighbor.Key == ignore) continue;

				ChainRefreshPaths(neighbor.Value, neighbor.Key.Opposite(), updateColorPath, updateShortestPath);
			}
		}

		/// <summary>
		/// This method adjust a newly added tile positioning and rotation
		/// </summary>
		/// <param name="direction"></param>
		/// <param name="rule"></param>
		/// <param name="tile"></param>
		private void AdjustNeighborSpace(CardinalDirection direction, NeighborWalkRule rule, Tile tile, Matrix4x4 root,
			out Vector3 translate, out Quaternion rotate)
		{
			tile.Index = _tile.Index;

			translate = Vector3.zero;
			rotate = Quaternion.identity;

			var rootRight = root.GetColumn(0).normalized;
			var rootForward = root.GetColumn(2).normalized;

			switch (direction)
			{
				case CardinalDirection.North:
					TranslateNorth(rule, tile, rootForward, rootRight, out translate, out rotate);
					CreateHinge(CardinalDirection.North, rule, tile);
					break;
				case CardinalDirection.South:
					TranslateSouth(rule, tile, rootForward, rootRight, out translate, out rotate);
					CreateHinge(CardinalDirection.South, rule, tile);
					break;
				case CardinalDirection.East:
					TranslateEast(rule, tile, rootForward, rootRight, out translate, out rotate);
					CreateHinge(CardinalDirection.East, rule, tile);
					break;
				case CardinalDirection.West:
					TranslateWest(rule, tile, rootForward, rootRight, out translate, out rotate);
					CreateHinge(CardinalDirection.West, rule, tile);
					break;
			}

			tile.Index += translate;
		}

		/// <summary>
		/// This method creates the hinge points necessary to join two tiles together, once
		/// these points have been calculated they can be used for translation
		/// </summary>
		/// <param name="direction">The tile insertion direction, useful to determine which
		/// face of the tile will be used to calculate the points
		/// </param>
		/// <param name="rule">The insertion rule, useful to determine if the points should
		/// be in the middle, middle up or middle down depending on the rule configuration
		/// </param>
		/// <param name="tile">The new tile</param>
		private void CreateHinge(CardinalDirection direction, NeighborWalkRule rule, Tile tile)
		{
			var forward = Vector3.zero;
			var up = Vector3.up;

			switch (direction)
			{
				case CardinalDirection.North:
					forward = Vector3.forward;
					break;
				case CardinalDirection.South:
					forward = Vector3.back;
					break;
				case CardinalDirection.East:
					forward = Vector3.right;
					break;
				case CardinalDirection.West:
					forward = Vector3.left;
					break;
			}

			// parent hinge
			_tile.HingePoints[direction] = _tile.Bounds.extents.z * forward;
			// inserted hinge
			tile.HingePoints[direction.Opposite()] = tile.Bounds.extents.z * -forward;

			if (rule == NeighborWalkRule.Down)
			{
				_tile.HingePoints[direction] -= _tile.Bounds.extents.y * up;
				tile.HingePoints[direction.Opposite()] -= tile.Bounds.extents.y * up;
			}
			else if (rule == NeighborWalkRule.Up)
			{
				_tile.HingePoints[direction] += _tile.Bounds.extents.y * up;
				tile.HingePoints[direction.Opposite()] += tile.Bounds.extents.y * up;
			}
		}

		private Vector3 TranslateWest(NeighborWalkRule rule, Tile tile, Vector3 forward, Vector3 right,
			out Vector3 translate, out Quaternion rotate)
		{
			// first take continuity behaviour, first the horizontal case:
			translate = -right;
			rotate = Quaternion.identity;

			if (rule == NeighborWalkRule.Down)
			{
				rotate = Quaternion.Euler(forward * 90);
				translate = translate - (rotate * right);
			}
			else if (rule == NeighborWalkRule.Up)
			{
				rotate = Quaternion.Euler(forward * -90);
				translate = translate - (rotate * right);
			}

			return translate;
		}

		private Vector3 TranslateEast(NeighborWalkRule rule, Tile tile, Vector3 forward, Vector3 right,
			out Vector3 translate, out Quaternion rotate)
		{
			// first take continuity behaviour, first the horizontal case:
			translate = right;
			rotate = Quaternion.identity;

			if (rule == NeighborWalkRule.Down)
			{
				rotate = Quaternion.Euler(forward * -90);
				translate = translate + (rotate * right);
			}
			else if (rule == NeighborWalkRule.Up)
			{
				rotate = Quaternion.Euler(forward * 90);
				translate = translate + (rotate * right);
			}

			return translate;
		}

		private void TranslateNorth(NeighborWalkRule rule, Tile tile, Vector3 forward, Vector3 right,
			out Vector3 translate, out Quaternion rotate)
		{
			// first take continuity behaviour, first the horizontal case:
			translate = forward;
			rotate = Quaternion.identity;

			if (rule == NeighborWalkRule.Down)
			{
				rotate = Quaternion.Euler(right * 90);
				translate = translate + (rotate * forward);
			}
			else if (rule == NeighborWalkRule.Up)
			{
				rotate = Quaternion.Euler(right * -90);
				translate = translate + (rotate * forward);
			}
		}

		private Vector3 TranslateSouth(NeighborWalkRule rule, Tile tile, Vector3 forward, Vector3 right,
			out Vector3 translate, out Quaternion rotate)
		{
			// first take continuity behaviour, first the horizontal case:
			translate = -forward;
			rotate = Quaternion.identity;

			if (rule == NeighborWalkRule.Down)
			{
				rotate = Quaternion.Euler(right * -90);
				translate = translate - (rotate * forward);
			}
			else if (rule == NeighborWalkRule.Up)
			{
				rotate = Quaternion.Euler(right * 90);
				translate = translate - (rotate * forward);
			}

			return translate;
		}

		/// <summary>
		/// Tile removal works by changing the color of this tile for the next tile color
		/// in its shortest path to a leaf tile, this action is then spread through all the
		/// tiles in the path
		/// </summary>
		public void Remove()
		{
			// obtain the path that should be updated after removal
			var shufflePath = _tile.ShortestPathToLeaf;

			if (shufflePath == null || shufflePath.Count == 0)
			{
				shufflePath = _tile.GetShortestLeafPath();

				if (shufflePath == null || shufflePath.Count == 0)
				{
					_tile.ShuffleColor();
					Debug.LogWarning("No possible shuffle path found for this tile");
					return;
				}

				shufflePath.Reverse();
			}

			// update the path
			for (int i = 0; i < shufflePath.Count; i++)
			{
				var source = shufflePath[i];

				// last tile obtains a new color
				if (i == shufflePath.Count - 1)
				{
					source.ShuffleColor();
					continue;
				}

				var nextTo = shufflePath[i + 1];

				source.TileColor = nextTo.TileColor;
			}

			ChainRefreshPaths(_tile, updateShortestPath: false);
		}

		/// <summary>
		/// Combo removal works by changing the colors for all the tiles within a matching
		/// color patch
		/// </summary>
		public void RemoveCombo()
		{
			// combo removals require at least three of the same color in the matching path
			if (_tile.MatchingColorPatch == null || _tile.MatchingColorPatch.Count <= 2)
			{
				Debug.LogWarning("A combo requires at least three matching color tiles together");
				return;
			}

			foreach (var tile in _tile.MatchingColorPatch)
			{
				tile.ShuffleColor();
			}

			ChainRefreshPaths(_tile, updateShortestPath: false);
		}

		/// <summary>
		/// Changes the tile's bounding box parameters, copies over the given bounds
		/// Only call this method when instancing a new tile.
		/// </summary>
		/// <param name="bounds"></param>
		public void AdjustBounds(Bounds bounds)
		{
			_tile.Bounds = bounds;
		}
	}
}