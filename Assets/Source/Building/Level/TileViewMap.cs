using System;
using System.Collections.Generic;
using System.Linq;
using TilesWalk.Gameplay.Level;
using TilesWalk.General;
using TilesWalk.General.Patterns;
using TilesWalk.Tile;
using TilesWalk.Tile.Level;
using TilesWalk.Tile.Rules;
using UniRx;
using UnityEngine;
using Zenject;
using LevelTileView = TilesWalk.Tile.Level.LevelTileView;

namespace TilesWalk.Building.Level
{
	public abstract class TileViewMap<T1, T2, T3> : MonoBehaviour where T1 : GenericMap where T2 : TileView where T3 : GenericFactory<T2>
	{
		[Inject] protected CustomLevelsConfiguration _customLevelsConfiguration;
		[Inject] protected T3 _factory;

		[TextArea, SerializeField] protected string _instructions;
		[SerializeField] protected T1 _map = default(T1);

		protected Dictionary<Tile.Tile, T2> TileView { get; } = new Dictionary<Tile.Tile, T2>();
		public Dictionary<T2, int> TileToHash { get; } = new Dictionary<T2, int>();
		public Dictionary<int, T2> HashToTile { get; } = new Dictionary<int, T2>();

		public Dictionary<int, List<InsertionInstruction>> Insertions { get; } =
			new Dictionary<int, List<InsertionInstruction>>();

		public T1 Map => _map;

		protected Subject<T1> _onLevelMapLoaded;
		protected Subject<T2> _onTileRegistered;
        protected Subject<T2> _onTileRemoved;
		protected Subject<TileLevelMapState> _onMapStateChanged;

		public IObservable<T1> OnLevelMapLoadedAsObservable()
		{
			return _onLevelMapLoaded = _onLevelMapLoaded ?? new Subject<T1>();
		}

		public IObservable<T2> OnTileRegisteredAsObservable()
		{
			return _onTileRegistered = _onTileRegistered ?? new Subject<T2>();
		}

        public IObservable<T2> OnTileRemovedAsObservable()
        {
            return _onTileRemoved = _onTileRemoved ?? new Subject<T2>();
        }

		public IObservable<TileLevelMapState> OnMapStateChangedAsObservable()
		{
			return _onMapStateChanged = _onMapStateChanged ?? new Subject<TileLevelMapState>();
		}

		protected virtual void Start()
		{
			_factory.OnNewInstanceAsObservable().Subscribe(OnNewTileInstance).AddTo(this);
		}

		protected virtual void OnDestroy()
		{
			_onLevelMapLoaded?.OnCompleted();
			_onTileRegistered?.OnCompleted();
			_onMapStateChanged?.OnCompleted();
            _onTileRemoved?.OnCompleted();
		}

		public void UpdateInstructions(T2 root, T2 tile, CardinalDirection d, NeighborWalkRule r)
		{
			if (!TileToHash.TryGetValue(root, out var rootId) ||
			    !TileToHash.TryGetValue(tile, out var tileId)) return;

			if (!Insertions.TryGetValue(rootId, out var insertions))
			{
				Insertions[rootId] = insertions = new List<InsertionInstruction>();
			}

			insertions.Add(new InsertionInstruction()
			{
				Tile = tileId,
				Root = rootId,
				Direction = d,
				Rule = r
			});

			_map.Instructions.Add(insertions.Last());
		}

		public virtual void Reset()
		{
			// reset data structures
			if (TileView.Count > 0)
			{
				foreach (var value in TileView.Values)
				{
					Destroy(value.transform.parent.gameObject);
				}
			}

			TileView.Clear();
			HashToTile.Clear();
			TileToHash.Clear();
			Insertions.Clear();
		}

		public bool IsBreakingDistance(LevelTileView tile)
		{
			var tiles = HashToTile.Values.ToList();
			var tileBounds = new Bounds
			(
				tile.transform.position,
				tile.Collider.size * (_customLevelsConfiguration.TileSeparationBoundsOffset)
			);

			return tiles.Any(x =>
			{
				var tightBound = new Bounds
				(
					x.transform.position,
					x.Collider.bounds.size * (_customLevelsConfiguration.TileSeparationBoundsOffset)
				);
				return tightBound.Intersects(tileBounds);
			});
		}

		public T2 GetTileView(Tile.Tile tile)
		{
			return TileView[tile];
		}

		public bool HasTileView(Tile.Tile tile)
		{
			return TileView.ContainsKey(tile);
		}

		public abstract void RemoveTile(T2 tile);

		/// <summary>
		/// Method for when a new tile is instanced but not registered yet
		/// </summary>
		/// <param name="tile"></param>
		protected abstract void OnNewTileInstance(T2 tile);

		/// <summary>
		/// This method finally registers a <see cref="TileView"/> to this the internal
		/// map structure, use this to confirm that a tile instance is part of the map
		/// </summary>
		/// <param name="tile"></param>
		/// <param name="hash"></param>
		public abstract void RegisterTile(T2 tile, int? hash = null);

		/// <summary>
		/// The map building method, here resides the logic for building the
		/// tile map
		/// </summary>
		/// <typeparam name="T">Generic for tile view, this enables
		/// the tile level map to build maps of other inheriting
		/// classes</typeparam>
		/// <param name="map"></param>
		public abstract void BuildTileMap<T>(T1 map) where T : T2;
	}
}