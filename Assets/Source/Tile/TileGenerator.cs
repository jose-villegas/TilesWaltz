using TilesWalk.BaseInterfaces;
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

			// Instantiate first tile
			var instance = Instantiate(source, Vector3.zero, Quaternion.identity, transform);
		}
	}
}


