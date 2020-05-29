using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UniRx;
using UnityEngine;

namespace TilesWalk.Tile
{
	public partial class TileView
	{
		private List<Tuple<Vector3, Quaternion>> ShufflePath(IReadOnlyList<TileView> shufflePath)
		{
			if (shufflePath == null || shufflePath.Count <= 0) return null;

			// this structure with backup the origin position and rotations
			var backup = new List<Tuple<Vector3, Quaternion>>();

			for (var i = 0; i < shufflePath.Count - 1; i++)
			{
				var source = shufflePath[i];
				var nextTo = shufflePath[i + 1];
				// backup info
				backup.Add(new Tuple<Vector3, Quaternion>(source.transform.position, source.transform.rotation));
				// copy transform
				source.transform.position = nextTo.transform.position;
				source.transform.rotation = nextTo.transform.rotation;
			}

			return backup;
		}

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

			var shufflePath = _controller.Remove();
			;

			if (shufflePath == null || shufflePath.Count <= 0) return;

			var tiles = shufflePath.Select(x => _viewFactory.GetTileView(x)).ToList();
			// this structure with backup the origin position and rotations
			var backup = ShufflePath(tiles);

			// since the last tile has no other to exchange positions with, reduce its
			// scale to hide it before showing its new color
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

		[Button]
		private void RemoveCombo()
		{
			if (MovementLocked)
			{
				Debug.LogWarning(
					"Tile movement is currently locked, wait for unlock for removal to be available again");
				return;
			}

			MovementLocked = true;

			var shufflePathCycles = _controller.RemoveCombo();
			StartCoroutine(RemoveComboCyclesRoutine(shufflePathCycles))
				.GetAwaiter()
				.OnCompleted(() => { MovementLocked = false; });
		}

		private IEnumerator RemoveComboCyclesRoutine(List<List<List<Tile>>> shufflePathCycles)
		{
			foreach (var cyclePaths in shufflePathCycles)
			{
				var pathReadyCount = 0;

				foreach (var shufflePath in cyclePaths)
				{
					var tiles = shufflePath.Select(x => _viewFactory.GetTileView(x)).ToList();
					// this structure with backup the origin position and rotations
					var backup = ShufflePath(tiles);

					// since the last tile has no other to exchange positions with, reduce its
					// scale to hide it before showing its new color
					var lastTile = _viewFactory.GetTileView(shufflePath[shufflePath.Count - 1]);
					var scale = lastTile.transform.localScale;
					lastTile.transform.localScale = Vector3.zero;

					StartCoroutine(ChainTowardsAnimation(tiles, backup))
						.GetAwaiter()
						.OnCompleted(() =>
						{
							StartCoroutine(lastTile.LastShuffleTileAnimation(scale))
								.GetAwaiter()
								.OnCompleted(() => pathReadyCount++);
						});
				}

				while (pathReadyCount < cyclePaths.Count) yield return new WaitForEndOfFrame();
			}
		}
	}
}