using System;
using System.Linq;
using Boo.Lang;
using TilesWalk.BaseInterfaces;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Display;
using TilesWalk.General;
using TilesWalk.Tile.Rules;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TilesWalk.Tile
{
	/// <summary>
	/// This class handles all the modifications over the <see cref="Tile"/>
	/// class model and its internal structure.
	/// </summary>
	[Serializable]
	public class TileController : IController
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
        /// <param name="translate">The translation needed for the neighbor tile view</param>
        /// <param name="rotate">The rotation needed for the neighboring tile view</param>
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

		/// <summary>
		/// Removes a neighboring tile relation with this tile
		/// </summary>
		/// <param name="direction">
		/// The direction where the neighboring
		/// tile to be removed can be found
		/// </param>
		public void RemoveNeighbor(CardinalDirection direction)
		{
			_tile.Neighbors.Remove(direction);
			_tile.HingePoints.Remove(direction);
			ChainRefreshPaths(_tile);
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

			tile.Index += new Vector3Int
			(
				Mathf.RoundToInt(translate.x),
				Mathf.RoundToInt(translate.y),
				Mathf.RoundToInt(translate.z)
			);
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

			var sourcePowerUp = _tile.PowerUp;
			var sourceColor = _tile.TileColor;
			_tile.PowerUp = TilePowerUp.None;


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
				source.PowerUp = nextTo.PowerUp;
			}

			// ignore otherwise as this should be handled by HandleTilePowerUp
			if (sourcePowerUp != TilePowerUp.None)
			{
				_tile.PowerUp = sourcePowerUp;

				if (sourcePowerUp == TilePowerUp.ColorMatch)
				{
					_tile.TileColor = sourceColor;
				}
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

			FindColorMatchPowerUp();

			foreach (var tile in _tile.MatchingColorPatch)
			{
				tile.ShuffleColor();
			}

			ChainRefreshPaths(_tile, updateShortestPath: false);
		}

		private void FindColorMatchPowerUp()
		{
			var powerUp = TilePowerUp.None;

			// check for combo power-ups
			if (_tile.MatchingColorPatch.Count >= 5)
			{
				// color match power-up
				_tile.MatchingColorPatch[Random.Range(0, _tile.MatchingColorPatch.Count)].PowerUp =
					TilePowerUp.ColorMatch;
			}
			else if (_tile.MatchingColorPatch.Count >= 4)
			{
				var cardinalDirection = CardinalDirection.None;
				var directionPowerUp = true;

				var first = _tile.MatchingColorPatch[0];
				// find its neighbor matching color
				var second = first.Neighbors.First(x => x.Value.TileColor == first.TileColor);
				// now we follow this direction to see if all the tiles match
				cardinalDirection = second.Key;

				for (int i = 1; i < _tile.MatchingColorPatch.Count; i++)
				{
					first = _tile.MatchingColorPatch[i];
					// find its neighbor matching color
					second = first.Neighbors.First(x => x.Value.TileColor == first.TileColor);

					if (second.Key != cardinalDirection && second.Key != cardinalDirection.Opposite())
					{
						cardinalDirection = CardinalDirection.None;
						break;
					}
				}

				// for balance the orthogonal axis is given, ensuring tiles will have to be
				// moved for maximizing
				if (directionPowerUp && cardinalDirection == CardinalDirection.South ||
				    cardinalDirection == CardinalDirection.North)
				{
					powerUp = TilePowerUp.EastWestLine;
				}
				else if (directionPowerUp && cardinalDirection == CardinalDirection.West ||
				         cardinalDirection == CardinalDirection.East)
				{
					powerUp = TilePowerUp.NorthSouthLine;
				}

				// assign to a tile randomly
				_tile.MatchingColorPatch[Random.Range(0, _tile.MatchingColorPatch.Count)].PowerUp = powerUp;
			}
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