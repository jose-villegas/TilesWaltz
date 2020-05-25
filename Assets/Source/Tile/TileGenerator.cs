using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Newtonsoft.Json;
using TilesWalk.BaseInterfaces;
using TilesWalk.General;
using TilesWalk.Tile.Rules;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace TilesWalk.Tile
{
	public class TileGenerator : MonoBehaviour, IGenerator<TileView>
	{
		[TextArea, SerializeField] private string _instructions;

		[Inject(Id = "TileAsset")] private AssetReference _tileAsset;
		[Inject] private DiContainer _container;

		private Dictionary<TileView, int> _tiles = new Dictionary<TileView, int>();
		private Dictionary<int, List<InsertionInstruction>> _insertions = new Dictionary<int, List<InsertionInstruction>>();

		private AsyncOperationHandle<GameObject> _asyncLoad;

		private void Start()
		{
			if (_tileAsset == null)
			{
				Debug.LogError("No asset assigned to the TileGenerator");
				return;
			}

			_asyncLoad = _tileAsset.LoadAssetAsync<GameObject>();
		}

		[Button]
		public TileView Generate()
		{
			if (_asyncLoad.Result == null)
			{
				Debug.LogError("No asset loaded for the TileGenerator");
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
			}

			return view;
		}

		public void UpdateInstructions(TileView tile, CardinalDirection d, NeighborWalkRule r)
		{
			int h;
			_tiles[tile] = tile.GetHashCode();

			if (_tiles.TryGetValue(tile, out h))
			{
				if (!_insertions.TryGetValue(h, out var insertions))
				{
					_insertions[h] = insertions = new List<InsertionInstruction>();
				}

				insertions.Add(new InsertionInstruction() {hash = h, direction = d, rule = r});
				_instructions = JsonConvert.SerializeObject(_insertions.Values.SelectMany(x => x));
			}
		}
	}
}