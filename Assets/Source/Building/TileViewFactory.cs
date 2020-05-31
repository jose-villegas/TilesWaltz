using TilesWalk.Building.Map;
using TilesWalk.General.Patterns;
using TilesWalk.Tile;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace TilesWalk.Building
{
	public class TileViewFactory : GenericFactory<TileView>
	{
		[Inject(Id = "TileAsset")] private AssetReference _tileAsset;
		[Inject] private DiContainer _container;

		public ReactiveProperty<bool> IsAssetLoaded { get; private set; } = new ReactiveProperty<bool>();
		
		private async void Start()
		{
			if (_tileAsset == null)
			{
				Debug.LogError("No asset assigned to the TileViewFactory");
				return;
			}

			// wait for load and assign factory asset
			var task = _tileAsset.LoadAssetAsync<GameObject>().Task;
			await task;
			Asset = task.Result;
			IsAssetLoaded.Value = true;
		}

		protected override TileView CreateInstance()
		{
			if (Asset == null)
			{
				Debug.LogError("No asset loaded for the TileViewFactory");
				return null;
			}

			// Instantiate first tile
			var instance = Instantiate(Asset, Vector3.zero, Quaternion.identity, transform);
			var view = _container.InstantiateComponent(typeof(TileView), instance) as TileView;

			// Obtain proper boundaries from collider
			if (view == null) return null;

			var boxCollider = view.GetComponent<BoxCollider>();
			view.Controller.AdjustBounds(boxCollider.bounds);
			view.Controller.Tile.ShuffleColor();
			
			return view;
		}
	}
}