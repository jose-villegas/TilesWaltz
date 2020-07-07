using TilesWalk.General.Patterns;
using UnityEngine;
using Zenject;

namespace TilesWalk.Map.Tile
{
	public class GameMapTileFactory : GenericFactory<GameMapTile>
	{
		[Inject(Id = "TileAsset", Optional = true)] private GameObject _tileAsset;

		private void Start()
		{
			if (_tileAsset == null)
			{
				Debug.LogWarning("Using direct asset assigned to the LevelTileViewFactory, injection couldn't be resolved");
				return;
			}

			Asset = _tileAsset;
		}

		protected override GameMapTile CreateInstance()
		{
			if (Asset == null)
			{
				Debug.LogError("No asset loaded for the LevelTileViewFactory");
				return null;
			}

			// Instantiate first tile
			var instance = Instantiate(Asset, Vector3.zero, Quaternion.identity, transform);
			var view = instance.GetComponent<GameMapTile>();

			// Obtain proper boundaries from collider
			if (view == null) return null;

			var boxCollider = view.Collider;
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
			var view = instance.GetComponent<T1>();

			// Obtain proper boundaries from collider
			if (view == null) return null;

			var boxCollider = view.Collider;
			view.Controller.AdjustBounds(boxCollider.bounds);
			view.Controller.Tile.ShuffleColor();

			return view;
		}
	}
}