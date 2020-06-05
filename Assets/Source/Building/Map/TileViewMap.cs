using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Newtonsoft.Json;
using TilesWalk.General;
using TilesWalk.Navigation.Map;
using TilesWalk.Tile;
using TilesWalk.Tile.Rules;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Building.Map
{
	public class TileViewMap : TileViewTrigger
	{
		[SerializeField] private TileViewMapStartLoadOptions _loadOption;
		[TextArea, SerializeField] private string _instructions;
		[SerializeField] TileMap _tileMap = new TileMap();

		[Inject] private TileViewFactory _viewFactory;
		[Inject] private MapLevelBridge _mapLevelBridge;

		private Dictionary<Tile.Tile, TileView> _tileView = new Dictionary<Tile.Tile, TileView>();

		public Dictionary<TileView, int> TileToHash { get; } = new Dictionary<TileView, int>();
		public Dictionary<int, TileView> HashToTile { get; } = new Dictionary<int, TileView>();

		public Dictionary<int, List<InsertionInstruction>> Insertions { get; } =
			new Dictionary<int, List<InsertionInstruction>>();

		public TileMap TileMap => _tileMap;

		protected Subject<Unit> _onTileMapLoaded;

		private void Start()
		{
			_viewFactory.OnNewInstanceAsObservable().Subscribe(OnNewTileInstance).AddTo(this);

			if (_loadOption == TileViewMapStartLoadOptions.FromInstructions)
			{
				_viewFactory.IsAssetLoaded.Subscribe(ready =>
				{
					if (ready) BuildFromInstructions();
				});
			}
			else if (_loadOption == TileViewMapStartLoadOptions.FromLevelBridge)
			{
				_viewFactory.IsAssetLoaded.Subscribe(ready =>
				{
					if (ready) BuildTileMap(_mapLevelBridge.SelectedTileMap);
				});
			}
		}

		private void OnNewTileInstance(TileView tile)
		{
			RegisterTile(tile);
			tile.OnComboRemovalAsObservable()
				.Subscribe(path => _onComboRemoval?.OnNext(path)).AddTo(this);
			tile.OnTileRemovedAsObservable()
				.Subscribe(path => _onTileRemoved?.OnNext(path)).AddTo(this);
		}

		public void RegisterTile(TileView tile, int? hash = null)
		{
			if (TileToHash.ContainsKey(tile))
			{
				var h = TileToHash[tile];
				TileToHash.Remove(tile);
				HashToTile.Remove(h);
			}

			var id = hash ?? tile.GetHashCode();
			TileToHash[tile] = id;
			HashToTile[id] = tile;
			_tileView[tile.Controller.Tile] = tile;
			// register tile to the tile map
			_tileMap.Tiles.Add(id);
		}

		public void RemoveTile(TileView tile)
		{
			if (!TileToHash.TryGetValue(tile, out var hash)) return;

			TileToHash.Remove(tile);
			HashToTile.Remove(hash);
			// remove from map
			_tileMap.Instructions.RemoveAll(x => x.tile == hash);
			_tileMap.Instructions.RemoveAll(x => x.root == hash);
			_tileMap.Tiles.Remove(hash);
			// remove all instructions that refer to this tile

			if (!Insertions.TryGetValue(hash, out var instructions)) return;

			Insertions.Remove(hash);

			foreach (var instruction in instructions)
			{
				Destroy(HashToTile[instruction.tile].gameObject);
			}
		}

		public TileView GetTileView(Tile.Tile tile)
		{
			return _tileView[tile];
		}

		[Button]
		public void RefreshAllPaths()
		{
			foreach (var viewKey in _tileView.Keys)
			{
				viewKey.RefreshShortestLeafPath();
				viewKey.RefreshMatchingColorPatch();
			}
		}

		[Button]
		public void GenerateInstructions()
		{
			var instr = Insertions.Values.SelectMany(x => x).ToList();
			var hashes = TileToHash.Values.ToList();

			var map = new TileMap()
			{
				Instructions = instr,
				Tiles = hashes
			};

			_instructions = JsonConvert.SerializeObject(map);
		}

		[Button]
		public void BuildFromInstructions()
		{
			// first instance all the needed tiles
			var map = JsonConvert.DeserializeObject<TileMap>(_instructions);
			BuildTileMap(map);
		}

		private void BuildTileMap(TileMap map)
		{
			// reset data structures
			HashToTile.Clear();
			TileToHash.Clear();
			Insertions.Clear();

			foreach (var mapTile in map.Tiles)
			{
				var tile = _viewFactory.NewInstance();
				// register with the source hash
				RegisterTile(tile, mapTile);
			}

			// Now execute neighbor insertion logic
			foreach (var instruction in map.Instructions)
			{
				var rootTile = HashToTile[instruction.root];
				var insert = HashToTile[instruction.tile];
				// adjust neighbor insertion
				rootTile.Controller.AddNeighbor(instruction.direction, instruction.rule, insert.Controller.Tile,
					rootTile.transform, insert.transform);
				UpdateInstructions(rootTile, insert, instruction.direction, instruction.rule);
			}

			_tileMap.Target = map.Target;
			_onTileMapLoaded?.OnCompleted();
		}

		public void UpdateInstructions(TileView root, TileView tile, CardinalDirection d, NeighborWalkRule r)
		{
			if (!TileToHash.TryGetValue(root, out var rootId) ||
			    !TileToHash.TryGetValue(tile, out var tileId)) return;

			if (!Insertions.TryGetValue(rootId, out var insertions))
			{
				Insertions[rootId] = insertions = new List<InsertionInstruction>();
			}

			insertions.Add(new InsertionInstruction()
			{
				tile = tileId,
				root = rootId,
				direction = d,
				rule = r
			});

			_tileMap.Instructions.Add(insertions.Last());
		}

		public IObservable<Unit> OnTileMapLoadedAsObservable()
		{
			return _onTileMapLoaded = _onTileMapLoaded ?? new Subject<Unit>();
		}
	}
}