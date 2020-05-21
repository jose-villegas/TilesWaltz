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

			var instances = new TileView[10];
			// Instantiate first tile
			for (int i = 0; i < 10; i++)
			{
				var instance = Instantiate(source, Vector3.zero, Quaternion.identity, transform);
				instances[i] = instance.AddComponent<TileView>();
			}
			// Add neighborhood structure
			for (int i = 0; i < 9; i++)
			{
				instances[i].Controller.AddNeighbor(CardinalDirection.North, instances[i + 1].Controller.Tile);
			}
		}
	}
}


