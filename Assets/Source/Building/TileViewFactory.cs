using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NaughtyAttributes;
using Newtonsoft.Json;
using TilesWalk.General;
using TilesWalk.Tile;
using TilesWalk.Tile.Rules;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace TilesWalk.Building
{
	public class TileViewFactory : MonoBehaviour, BaseInterfaces.IFactory<TileView>
	{
		[TextArea, SerializeField] private string _instructions;

		[Inject(Id = "TileAsset")] private AssetReference _tileAsset;
		[Inject] private DiContainer _container;

		private Dictionary<Tile.Tile, TileView> _tileView = new Dictionary<Tile.Tile, TileView>();

		private Dictionary<TileView, int> _tileToHash = new Dictionary<TileView, int>();
		private Dictionary<int, TileView> _hashToTile = new Dictionary<int, TileView>();

		private Dictionary<int, List<InsertionInstruction>> _insertions =
			new Dictionary<int, List<InsertionInstruction>>();

		private AsyncOperationHandle<GameObject> _asyncLoad;

		private void Start()
		{
			if (_tileAsset == null)
			{
				Debug.LogError("No asset assigned to the TileViewFactory");
				return;
			}

			_asyncLoad = _tileAsset.LoadAssetAsync<GameObject>();
		}

		public TileView GetTileView(Tile.Tile tile)
		{
			return _tileView[tile];
		}

		private void RegisterTile(TileView tile, int? hash = null)
		{
			if (_tileToHash.ContainsKey(tile))
			{
				var h = _tileToHash[tile];
				_tileToHash.Remove(tile);
				_hashToTile.Remove(h);
			}

			var id = hash ?? tile.GetHashCode();
			_tileToHash[tile] = id;
			_hashToTile[id] = tile;
			_tileView[tile.Controller.Tile] = tile;
		}

		[Button]
		public TileView NewInstance()
		{
			if (_asyncLoad.Result == null)
			{
				Debug.LogError("No asset loaded for the TileViewFactory");
				return null;
			}

			var source = _asyncLoad.Result;

			// Instantiate first tile
			var instance = Instantiate(source, Vector3.zero, Quaternion.identity, transform);
			var view = _container.InstantiateComponent(typeof(TileView), instance) as TileView;

			// Obtain proper boundaries from collider
			if (view != null)
			{
				var boxCollider = view.GetComponent<BoxCollider>();
				view.Controller.AdjustBounds(boxCollider.bounds);
				view.Controller.Tile.ShuffleColor();
				RegisterTile(view);
				// unregistered when destroyed
				view.OnDestroyAsObservable().Subscribe(x =>
				{
					if (!_tileToHash.TryGetValue(view, out var hash)) return;

					_tileToHash.Remove(view);
					_hashToTile.Remove(hash);
					// remove all instructions that refer to this tile

					if (!_insertions.TryGetValue(hash, out var instructions)) return;

					_insertions.Remove(hash);

					foreach (var instruction in instructions)
					{
						Destroy(_hashToTile[instruction.tile].gameObject);
					}
				}).AddTo(this);
			}

			return view;
		}

		[Button]
		public void GenerateInstructions()
		{
			var instr = _insertions.Values.SelectMany(x => x).ToList();
			var hashes = _tileToHash.Values.ToList();
			var allTiles = new Dictionary<int, Vector3>();

			foreach (var hash in hashes)
			{
				allTiles[hash] = _hashToTile[hash].Controller.Tile.Index;
			}

			var map = new TileMap()
			{
				instructions = instr,
				tiles = allTiles
			};

			_instructions = JsonConvert.SerializeObject(map);
		}

		[Button]
		public void BuildFromInstructions()
		{
			// reset data structures
			_hashToTile.Clear();
			_tileToHash.Clear();
			_insertions.Clear();
			// first instance all the needed tiles
			var map = JsonConvert.DeserializeObject<TileMap>(_instructions);

			foreach (var mapTile in map.tiles)
			{
				var tile = NewInstance();
				// register with the source hash
				RegisterTile(tile, mapTile.Key);
			}

			// Now execute neighbor insertion logic
			foreach (var instruction in map.instructions)
			{
				var rootTile = _hashToTile[instruction.root];
				var insert = _hashToTile[instruction.tile];
				// adjust neighbor insertion
				rootTile.Controller.AddNeighbor(instruction.direction, instruction.rule, insert.Controller.Tile,
					rootTile.transform, insert.transform);
				UpdateInstructions(rootTile, insert, instruction.direction, instruction.rule);
			}
		}

		public void UpdateInstructions(TileView root, TileView tile, CardinalDirection d, NeighborWalkRule r)
		{
			if (!_tileToHash.TryGetValue(root, out var rootId) ||
			    !_tileToHash.TryGetValue(tile, out var tileId)) return;

			if (!_insertions.TryGetValue(rootId, out var insertions))
			{
				_insertions[rootId] = insertions = new List<InsertionInstruction>();
			}

			insertions.Add(new InsertionInstruction()
			{
				tile = tileId,
				root = rootId,
				direction = d,
				rule = r
			});
		}
	}
}