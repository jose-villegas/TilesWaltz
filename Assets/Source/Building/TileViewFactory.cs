using TilesWalk.General.Patterns;
using TilesWalk.Tile;
using UnityEngine;
using Zenject;

namespace TilesWalk.Building
{
	public class TileViewFactory : GenericFactory<TileView>
	{
		[Inject(Id = "TileAsset")] private GameObject _tileAsset;
		[Inject] private DiContainer _container;

		private void Start()
		{
			if (_tileAsset == null)
			{
				Debug.LogError("No asset assigned to the TileViewFactory");
				return;
			}

			Asset = _tileAsset;
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