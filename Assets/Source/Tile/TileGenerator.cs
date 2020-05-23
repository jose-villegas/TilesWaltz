using TilesWalk.BaseInterfaces;
using TilesWalk.General;
using TilesWalk.Tile.Rules;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace TilesWalk.Tile
{
	public class TileGenerator : MonoBehaviour, IGenerator<GameObject>
	{
		[Inject(Id = "TileAsset")]
		private AssetReference _tileAsset;
		[Inject] DiContainer 
		_container;

		private void Start()
		{
			if (_tileAsset == null)
			{
				Debug.LogError("No asset assigned to the TileGenerator");
				return;
			}

			var asyncLoad = _tileAsset.LoadAssetAsync<GameObject>();
			asyncLoad.Completed += OnAssetLoadCompleted;
		}

		private void OnAssetLoadCompleted(AsyncOperationHandle<GameObject> handle)
		{
			Generate(handle.Result);
		}

		public void Generate(GameObject source)
		{
			if (source == null)
			{
				Debug.LogError("No asset loaded for the TileGenerator");
				return;
			}

			var n = 1;
			var instances = new TileView[n];
			// Instantiate first tile
			for (int i = 0; i < n; i++)
			{
				var instance = Instantiate(source, Vector3.zero, Quaternion.identity, transform);
				instances[i] = _container.InstantiateComponent(typeof(TileView), instance) as TileView;

				// Obtain proper boundaries from collider
				var boxCollider = instances[i].GetComponent<BoxCollider>();
				instances[i].Controller.AdjustBounds(boxCollider.bounds);

			}
			// Add neighborhood structure
			//instances[0].Controller.AddNeighbor(CardinalDirection.North, NeighborWalkRule.Plain, instances[1].Controller.Tile);
			//instances[1].Controller.AddNeighbor(CardinalDirection.North, NeighborWalkRule.Plain, instances[2].Controller.Tile);
			//instances[2].Controller.AddNeighbor(CardinalDirection.West, NeighborWalkRule.Plain, instances[3].Controller.Tile);
			//instances[3].Controller.AddNeighbor(CardinalDirection.North, NeighborWalkRule.Plain, instances[4].Controller.Tile);
			//instances[4].Controller.AddNeighbor(CardinalDirection.North, NeighborWalkRule.Plain, instances[5].Controller.Tile);
			//instances[5].Controller.AddNeighbor(CardinalDirection.North, NeighborWalkRule.Plain, instances[6].Controller.Tile);
			//instances[6].Controller.AddNeighbor(CardinalDirection.North, NeighborWalkRule.Plain, instances[7].Controller.Tile);
			//instances[7].Controller.AddNeighbor(CardinalDirection.North, NeighborWalkRule.Plain, instances[8].Controller.Tile);
			//instances[8].Controller.AddNeighbor(CardinalDirection.West, NeighborWalkRule.Plain, instances[9].Controller.Tile);
			//instances[9].Controller.AddNeighbor(CardinalDirection.South, NeighborWalkRule.Plain, instances[10].Controller.Tile);
			//instances[10].Controller.AddNeighbor(CardinalDirection.South, NeighborWalkRule.Plain, instances[11].Controller.Tile);
			//instances[11].Controller.AddNeighbor(CardinalDirection.South, NeighborWalkRule.Down, instances[12].Controller.Tile);
			//instances[12].Controller.AddNeighbor(CardinalDirection.South, NeighborWalkRule.Down, instances[13].Controller.Tile);
			//instances[12].Controller.AddNeighbor(CardinalDirection.West, NeighborWalkRule.Down, instances[13].Controller.Tile);
		}
	}
}


