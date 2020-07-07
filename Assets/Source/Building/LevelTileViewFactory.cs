using TilesWalk.General.Patterns;
using TilesWalk.Tile;
using TilesWalk.Tile.Level;
using UnityEngine;
using Zenject;

namespace TilesWalk.Building
{
	public class LevelTileViewFactory : GenericFactory<LevelTileView>
	{
		[Inject(Id = "TileAsset", Optional = true)] private GameObject _tileAsset;
		[Inject] private DiContainer _container;

		private void Start()
		{
			if (_tileAsset == null)
			{
				Debug.LogWarning("Using direct asset assigned to the LevelTileViewFactory, injection couldn't be resolved");
				return;
			}

			Asset = _tileAsset;
		}

		protected override LevelTileView CreateInstance()
		{
			if (Asset == null)
			{
				Debug.LogError("No asset loaded for the LevelTileViewFactory");
				return null;
			}

			// Instantiate first tile
			var instance = Instantiate(Asset, Vector3.zero, Quaternion.identity, transform);
			var view = _container.InstantiateComponent(typeof(LevelTileView), instance) as LevelTileView;

			// Obtain proper boundaries from collider
			if (view == null) return null;

			var boxCollider = view.GetComponent<BoxCollider>();
			view.Controller.AdjustBounds(boxCollider.bounds);
			view.Controller.Tile.ShuffleColor();
			
			return view;
		}

		protected override T1 CreateInstance<T1>()
		{
			if (Asset == null)
			{
				Debug.LogError("No asset loaded for the LevelTileViewFactory");
				return null;
			}

			// Instantiate first tile
			var instance = Instantiate(Asset, Vector3.zero, Quaternion.identity, transform);

			// find model children to instance the tile view there
			var mesh = instance.GetComponentInChildren<MeshRenderer>();
			var view = _container.InstantiateComponent(typeof(T1), mesh.gameObject) as T1;

			// Obtain proper boundaries from collider
			if (view == null) return null;

			var boxCollider = view.Collider;
			view.Controller.AdjustBounds(boxCollider.bounds);
			view.Controller.Tile.ShuffleColor();

			return view;
		}
	}
}