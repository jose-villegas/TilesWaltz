using System;
using System.Collections.Generic;
using System.Linq;
using TilesWalk.BaseInterfaces;
using TilesWalk.Extensions;
using TilesWalk.General;
using TilesWalk.Tile.Rules;
using UniRx;
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

		/// <summary>
		/// This methods inserts a neighbor in the data structure of the tile path
		/// </summary>
		/// <param name="direction">The direction respective to this tile as root</param>
		/// <param name="rule">The insertion rule for this tile</param>
		/// <param name="tile">The tile data, to be referenced</param>
		/// <param name="rootTransform">Transform of the root tile</param>
		/// <param name="tileTransform">Transform of the new tile</param>
		public void AddNeighbor(CardinalDirection direction, NeighborWalkRule rule, Tile tile, Transform rootTransform,
			Transform tileTransform)
		{
			// copy neighbor
			tileTransform.position = rootTransform.position;
			tileTransform.rotation = rootTransform.rotation;
			tileTransform.localScale = rootTransform.localScale;
			// connect neighbor references
			_tile.Neighbors[direction] = tile;
			tile.Neighbors[direction.Opposite()] = _tile;
			// adjust 3d index according to neighbor
			AdjustNeighborSpace(direction, rule, tile, rootTransform, tileTransform);
			// refresh shortest path for all related neighbors
			ChainRefreshPaths(tile);
		}

		public void ChainRefreshPaths(Tile source, CardinalDirection ignore = CardinalDirection.None,
			bool updateColorPath = true, bool updateShortestPath = true)
		{
			if (updateColorPath) source.RefreshMatchingColorPath();
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
		/// <param name="rootTransform"></param>
		/// <param name="tileTransform"></param>
		private void AdjustNeighborSpace(CardinalDirection direction, NeighborWalkRule rule, Tile tile,
			Transform rootTransform, Transform tileTransform)
		{
			tile.Index = _tile.Index;

			Vector3 sourcePoint = default;
			Vector3 targetPoint = default;
			Vector3 translate = default;

			switch (direction)
			{
				case CardinalDirection.North:
					translate = TranslateNorth(rule, tile, tileTransform);
					tile.Index += translate;
					CreateHinge(CardinalDirection.North, rule, tile, rootTransform, tileTransform,
						rootTransform.forward, tileTransform.forward);
					sourcePoint = _tile.HingePoints[CardinalDirection.North];
					targetPoint = tile.HingePoints[CardinalDirection.South];
					break;
				case CardinalDirection.South:
					translate = TranslateSouth(rule, tile, tileTransform);
					tile.Index += translate;
					CreateHinge(CardinalDirection.South, rule, tile, rootTransform, tileTransform,
						-rootTransform.forward, -tileTransform.forward);
					sourcePoint = _tile.HingePoints[CardinalDirection.South];
					targetPoint = tile.HingePoints[CardinalDirection.North];
					break;
				case CardinalDirection.East:
					translate = TranslateEast(rule, tile, tileTransform);
					tile.Index += translate;
					CreateHinge(CardinalDirection.East, rule, tile, rootTransform, tileTransform, rootTransform.right,
						tileTransform.right);
					sourcePoint = _tile.HingePoints[CardinalDirection.East];
					targetPoint = tile.HingePoints[CardinalDirection.West];
					break;
				case CardinalDirection.West:
					translate = TranslateWest(rule, tile, tileTransform);
					tile.Index += translate;
					CreateHinge(CardinalDirection.West, rule, tile, rootTransform, tileTransform, -rootTransform.right,
						-tileTransform.right);
					sourcePoint = _tile.HingePoints[CardinalDirection.West];
					targetPoint = tile.HingePoints[CardinalDirection.East];
					break;
			}

			tileTransform.position = rootTransform.position + translate;
			tileTransform.position += sourcePoint - targetPoint;
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
		/// <param name="rootTransform">The root tile transform</param>
		/// <param name="tileTransform">The new tile transform</param>
		/// <param name="forward">This structure should contain two directional <see cref="Vector3"/>
		/// the first being the up/down rotation axis for the root tile, the other for the new tile
		/// </param>
		private void CreateHinge(CardinalDirection direction, NeighborWalkRule rule, Tile tile, Transform rootTransform,
			Transform tileTransform, params Vector3[] forward)
		{
			// parent hinge
			_tile.HingePoints[direction] = _tile.Index;
			_tile.HingePoints[direction] += _tile.Bounds.extents.z * forward[0];
			// inserted hinge
			tile.HingePoints[direction.Opposite()] = tile.Index;
			tile.HingePoints[direction.Opposite()] -= tile.Bounds.extents.z * forward[1];

			if (rule == NeighborWalkRule.Down)
			{
				_tile.HingePoints[direction] -= _tile.Bounds.extents.y * rootTransform.up;
				tile.HingePoints[direction.Opposite()] -= tile.Bounds.extents.y * tileTransform.up;
			}
			else if (rule == NeighborWalkRule.Up)
			{
				_tile.HingePoints[direction] += _tile.Bounds.extents.y * rootTransform.up;
				tile.HingePoints[direction.Opposite()] += tile.Bounds.extents.y * tileTransform.up;
			}
		}

		private Vector3 TranslateWest(NeighborWalkRule rule, Tile tile, Transform tileTransform)
		{
			// first take continuity behaviour, first the horizontal case:
			var translate = -tileTransform.right;

			if (rule == NeighborWalkRule.Down)
			{
				tileTransform.Rotate(tileTransform.forward, 90, Space.World);
				translate = translate - tileTransform.right;
			}
			else if (rule == NeighborWalkRule.Up)
			{
				tileTransform.Rotate(tileTransform.forward, -90, Space.World);
				translate = translate - tileTransform.right;
			}

			return translate;
		}

		private Vector3 TranslateEast(NeighborWalkRule rule, Tile tile, Transform tileTransform)
		{
			// first take continuity behaviour, first the horizontal case:
			var translate = tileTransform.right;

			if (rule == NeighborWalkRule.Down)
			{
				tileTransform.Rotate(tileTransform.forward, -90, Space.World);
				translate = translate + tileTransform.right;
			}
			else if (rule == NeighborWalkRule.Up)
			{
				tileTransform.Rotate(tileTransform.forward, 90, Space.World);
				translate = translate + tileTransform.right;
			}

			return translate;
		}

		private Vector3 TranslateNorth(NeighborWalkRule rule, Tile tile, Transform tileTransform)
		{
			// first take continuity behaviour, first the horizontal case:
			var translate = tileTransform.forward;

			if (rule == NeighborWalkRule.Down)
			{
				tileTransform.Rotate(tileTransform.right, 90, Space.World);
				translate = translate + tileTransform.forward;
			}
			else if (rule == NeighborWalkRule.Up)
			{
				tileTransform.Rotate(tileTransform.right, -90, Space.World);
				translate = translate + tileTransform.forward;
			}

			return translate;
		}

		private Vector3 TranslateSouth(NeighborWalkRule rule, Tile tile, Transform tileTransform)
		{
			// first take continuity behaviour, first the horizontal case:
			var translate = -tileTransform.forward;

			if (rule == NeighborWalkRule.Down)
			{
				tileTransform.Rotate(tileTransform.right, -90, Space.World);
				translate = translate - tileTransform.forward;
			}
			else if (rule == NeighborWalkRule.Up)
			{
				tileTransform.Rotate(tileTransform.right, 90, Space.World);
				translate = translate - tileTransform.forward;
			}

			return translate;
		}

		/// <summary>
		/// Removes this tile from the tile structure
		/// </summary>
		public List<Tile> Remove()
		{
			// obtain the path that should be updated after removal
			var shufflePath = _tile.ShortestPathToLeaf;

			if (shufflePath == null || shufflePath.Count == 0)
			{
				shufflePath = _tile.GetShortestLeafPath();
				shufflePath.Reverse();
			}

			if (shufflePath == null || shufflePath.Count == 0)
			{
				_tile.ShuffleColor();
				Debug.LogWarning("No possible shuffle path found for this tile");
				return shufflePath;
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
			return shufflePath;
		}

		public List<List<List<Tile>>> RemoveCombo()
		{
			// combo removals require at least three of the same color in the matching path
			if (_tile.MatchingColorPatch == null || _tile.MatchingColorPatch.Count <= 2)
			{
				Debug.LogWarning("A combo requires at least three matching color tiles together");
				return null;
			}

			var matchingColor = _tile.TileColor;

			var leafs = _tile.MatchingColorPatch.Where(x => x.IsColorLeaf() && x.TileColor == matchingColor).ToList();
			var result = new List<List<List<Tile>>>();

			while (leafs.Count >= 1)
			{
				// add new cycle
				result.Add(new List<List<Tile>>());
				// unlike common removal the shuffle path here will choose the opposite shortest path
				// to the lef tiles, instead of the shortest path, this avoids paths that would shuffle
				// the colors within the matching color path
				foreach (var leaf in leafs)
				{
					var shufflePath = leaf.GetShortestLeafPath(leaf.Neighbors
						.Where(x => x.Value.TileColor != matchingColor)
						.Select(x => x.Key.Opposite()).ToArray());

					if (shufflePath == null || shufflePath.Count == 0)
					{
						_tile.ShuffleColor();
						continue;
					}

					shufflePath.Reverse();
					// add this leaf path to the latest cycle
					result[result.Count - 1].Add(shufflePath);

					// update the path
					for (var i = 0; i < shufflePath.Count; i++)
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
				}

				// update leaf array with new leafs after color shuffle
				leafs = _tile.MatchingColorPatch.Where(x => x.IsColorLeaf() && x.TileColor == matchingColor).ToList();
			}

			ChainRefreshPaths(_tile, updateShortestPath: false);
			return result;
		}

		internal void AdjustBounds(Bounds bounds)
		{
			_tile.Bounds = bounds;
		}
	}
}