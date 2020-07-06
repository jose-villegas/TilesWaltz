using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TilesWalk.BaseInterfaces;
using TilesWalk.Extensions;
using TilesWalk.Gameplay;
using TilesWalk.Gameplay.Display;
using TilesWalk.General;
using TilesWalk.Tile.Rules;
using UniRx;
using UnityEngine;

namespace TilesWalk.Tile
{
	/// <summary>
	/// Tile class, contains all properties and fields related to the in-game tiles
	/// puzzle figure, most property names are self explanatory
	/// </summary>
	[Serializable]
	public class Tile : IModel
	{
		[SerializeField] private Vector3Int _index;
		[SerializeField] private Bounds _bounds;
		[SerializeField] private TileColor _color;
		[SerializeField] private TilePowerUp _powerUp;

		private Subject<Tuple<Tile, TileColor>> _onTileColorChanged;
		private Subject<Tuple<Tile, TilePowerUp>> onTilePowerUpChanged;

		public bool Root { get; set; }

		/// <summary>
		/// This structure contains a reference to the neighbor tiles, useful for indexing
		/// the structure, each index represents an index at <see cref="CardinalDirection"/>
		/// </summary>
		public Dictionary<CardinalDirection, Tile> Neighbors { get; set; }

		/// <summary>
		/// This points connects this tile with the neighbor tile, useful for positioning
		/// </summary>
		public Dictionary<CardinalDirection, Vector3> HingePoints { get; set; }

		/// <summary>
		/// This vector contains a 3D coordinate respective to the tile structure, though visually
		/// it doesn't look like a series of voxels, this coordinate represents its position in voxel
		/// space
		/// </summary>
		public Vector3Int Index
		{
			get => _index;
			set => _index = value;
		}

		public Bounds Bounds
		{
			get => _bounds;
			set => _bounds = value;
		}

		public TileColor TileColor
		{
			get => _color;
			set
			{
				var sourceColor = _color;
				_color = value;
				_onTileColorChanged?.OnNext(new Tuple<Tile, TileColor>(this, sourceColor));
			}
		}

		public List<Tile> ShortestPathToLeaf { get; private set; }
		public List<Tile> MatchingColorPatch { get; private set; }

		public TilePowerUp PowerUp
		{
			get => _powerUp;
			set
			{
				var sourcePower = _powerUp;
				_powerUp = value;
				onTilePowerUpChanged?.OnNext(new Tuple<Tile, TilePowerUp>(this, sourcePower));
			}
		}

		public void ShuffleColor(bool self = false)
		{
			var distinct = Neighbors.Select(x => x.Value.TileColor).ToList();

			if (self) distinct.Add(TileColor);

			TileColor = TileColorExtension.RandomColor(distinct.ToArray());
		}

		public void RefreshShortestLeafPath()
		{
			ShortestPathToLeaf = this.GetShortestLeafPath();
			ShortestPathToLeaf.Reverse();
		}

		public void RefreshMatchingColorPatch()
		{
			MatchingColorPatch = this.GetColorMatchPatch();
		}

		public Tile()
		{
			_color = TileColor.None;
			_bounds = new Bounds();
			_index = Vector3Int.zero;
			HingePoints = new Dictionary<CardinalDirection, Vector3>();
			Neighbors = new Dictionary<CardinalDirection, Tile>();
		}

		public IObservable<Tuple<Tile, TileColor>> OnTileColorChangedAsObservable()
		{
			return _onTileColorChanged = _onTileColorChanged == null
				? new Subject<Tuple<Tile, TileColor>>()
				: _onTileColorChanged;
		}

		public IObservable<Tuple<Tile, TilePowerUp>> OnTilePowerUpChangedAsObservable()
		{
			return onTilePowerUpChanged = onTilePowerUpChanged == null
				? new Subject<Tuple<Tile, TilePowerUp>>()
				: onTilePowerUpChanged;
		}

		public List<Tile> GetAllOfColor()
		{
			var result = new List<Tile>() {this};

			foreach (var neighbor in Neighbors)
			{
				result.AddRange(GetAllOfColor(neighbor.Value, TileColor, neighbor.Key.Opposite()));
			}

			result.Sort((t1, t2) =>
			{
				var dst1 = (Index - t1.Index).sqrMagnitude;
				var dst2 = (Index - t2.Index).sqrMagnitude;
				return dst1 - dst2;
			});

			//if (applyPowerModifier)
			//{
			//	float numberOfColors = Neighbors.Count(x => x.Value.TileColor == TileColor);

			//	// neighboring colors manage the potency of the power up
			//	if (numberOfColors < 1)
			//	{
			//		var percent = result.Count - Mathf.CeilToInt(result.Count * 0.33f);
			//		result.RemoveRange(result.Count - percent, percent);
			//	}
			//	else if (numberOfColors >= 1 && numberOfColors < 2)
			//	{
			//		var percent = result.Count - Mathf.CeilToInt(result.Count * 0.66f);
			//		result.RemoveRange(result.Count - percent, percent);
			//	}
			//}

			return result;
		}

		private static List<Tile> GetAllOfColor(Tile source, TileColor color, CardinalDirection ignore)
		{
			if (source.IsLeaf())
			{
				return null;
			}

			var result = new List<Tile>();

			foreach (var neighbor in source.Neighbors)
			{
				if (neighbor.Key == ignore) continue;

				if (neighbor.Value.TileColor == color) result.Add(neighbor.Value);

				var neighborColors = GetAllOfColor(neighbor.Value, color, neighbor.Key.Opposite());

				if (neighborColors != null && neighborColors.Count > 0)
				{
					result.AddRange(neighborColors);
				}
			}

			return result;
		}

		public List<Tile> GetStraightPath(bool applyPowerModifier, params CardinalDirection[] direction)
		{
			var sourceTile = this;
			var result = new List<Tile>() {sourceTile};

			foreach (var cardinalDirection in direction)
			{
				sourceTile = this;
				while (sourceTile.Neighbors.TryGetValue(cardinalDirection, out var currentTile))
				{
					sourceTile = currentTile;
					result.Add(currentTile);
				}
			}

			result.Sort((t1, t2) =>
			{
				var dst1 = (Index - t1.Index).sqrMagnitude;
				var dst2 = (Index - t2.Index).sqrMagnitude;
				return dst1 - dst2;
			});

			if (applyPowerModifier)
			{
				float numberOfColors = Neighbors.Count(x => x.Value.TileColor == TileColor);

				// neighboring colors manage the potency of the power up
				if (numberOfColors < 1)
				{
					var percent = result.Count - Mathf.CeilToInt(result.Count * 0.33f);
					result.RemoveRange(result.Count - percent, percent);
				}
				else if (numberOfColors >= 1 && numberOfColors < 2)
				{
					var percent = result.Count - Mathf.CeilToInt(result.Count * 0.66f);
					result.RemoveRange(result.Count - percent, percent);
				}
			}


			return result;
		}
	}
}