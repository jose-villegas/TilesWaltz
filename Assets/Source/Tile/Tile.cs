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

		private Subject<Tuple<Tile, TileColor>> _onTileColorChanged;

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

		public void ShuffleColor()
		{
			TileColor = TileColorExtension.RandomColor(this.Neighbors.Select(x => x.Value.TileColor).ToArray());
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
	}
}