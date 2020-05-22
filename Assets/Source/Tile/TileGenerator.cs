using TilesWalk.BaseInterfaces;
using TilesWalk.General;
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

			var instances = new TileView[30];
			// Instantiate first tile
			for (int i = 0; i < 30; i++)
			{
				var instance = Instantiate(source, Vector3.zero, Quaternion.identity, transform);
				instances[i] = instance.AddComponent<TileView>();

				// Obtain proper boundaries from collider
				var boxCollider = instances[i].GetComponent<BoxCollider>();
				instances[i].Controller.AdjustBounds(boxCollider.bounds);

			}
			// Add neighborhood structure
			for (int i = 0; i < 6; i++)
			{
				if ((i + 1) % 3 == 0)
					instances[i].Controller.AddNeighbor(CardinalDirection.North, NeighborWalkRule.Plain, instances[i + 1].Controller.Tile);
				else
					instances[i].Controller.AddNeighbor(CardinalDirection.North, NeighborWalkRule.Up, instances[i + 1].Controller.Tile);
			}

			instances[6].Controller.AddNeighbor(CardinalDirection.North, NeighborWalkRule.Plain, instances[7].Controller.Tile);
			instances[7].Controller.AddNeighbor(CardinalDirection.West, NeighborWalkRule.Plain, instances[8].Controller.Tile);
			instances[8].Controller.AddNeighbor(CardinalDirection.West, NeighborWalkRule.Plain, instances[9].Controller.Tile);
			instances[8].Controller.AddNeighbor(CardinalDirection.West, NeighborWalkRule.Down, instances[9].Controller.Tile);
		}
	}
}


