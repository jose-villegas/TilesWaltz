using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UniRx;
using UnityEngine;

namespace TilesWalk.Tile
{
	public partial class TileView
	{
		[Button]
		private void Remove()
		{
			if (MovementLocked)
			{
				Debug.LogWarning(
					"Tile movement is currently locked, wait for unlock for removal to be available again");
				return;
			}

			MovementLocked = true;

			_controller.Remove();

			List<Tile> shufflePath = _controller.Tile.ShortestPathToLeaf;

			if (shufflePath == null || shufflePath.Count <= 0) return;

			// this structure with backup the origin position and rotations
			var backup = new List<Tuple<Vector3, Quaternion>>();
			var tiles = new List<TileView>();

			for (int i = 0; i < shufflePath.Count - 1; i++)
			{
				var source = _viewFactory.GetTileView(shufflePath[i]);
				var nextTo = _viewFactory.GetTileView(shufflePath[i + 1]);
				// backup info
				backup.Add(new Tuple<Vector3, Quaternion>(source.transform.position, source.transform.rotation));
				tiles.Add(source);
				// copy transform
				source.transform.position = nextTo.transform.position;
				source.transform.rotation = nextTo.transform.rotation;
			}

			var lastTile = _viewFactory.GetTileView(shufflePath[shufflePath.Count - 1]);
			var scale = lastTile.transform.localScale;
			lastTile.transform.localScale = Vector3.zero;

			StartCoroutine(ChainTowardsAnimation(tiles, backup))
				.GetAwaiter()
				.OnCompleted(() =>
				{
					StartCoroutine(lastTile.LastShuffleTileAnimation(scale))
						.GetAwaiter()
						.OnCompleted(() => MovementLocked = false);
				});
		}
	}
}