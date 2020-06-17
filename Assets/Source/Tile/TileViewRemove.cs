using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UniRx;
using UniRx.Triggers;
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

			_controller.Remove();
			var shufflePath = _controller.Tile.ShortestPathToLeaf;

			if (shufflePath == null || shufflePath.Count <= 0) return;

			var tiles = shufflePath.Select(x => _tileLevelMap.GetTileView(x)).ToList();
			// this structure with backup the origin position and rotations
			var backup = ShufflePath(tiles);

			// since the last tile has no other to exchange positions with, reduce its
			// scale to hide it before showing its new color
			var lastTile = _tileLevelMap.GetTileView(shufflePath[shufflePath.Count - 1]);
			var scale = lastTile.transform.localScale;
			lastTile.transform.localScale = Vector3.zero;

			MainThreadDispatcher.StartUpdateMicroCoroutine(ShuffleMoveAnimation(tiles, backup));
			Observable.Timer(TimeSpan.FromSeconds(_animationSettings.ShuffleMoveTime)).Subscribe(_ => { }, () =>
			{
				MainThreadDispatcher.StartUpdateMicroCoroutine(lastTile.ScalePopInAnimation(scale));
				Observable.Timer(TimeSpan.FromSeconds(_animationSettings.ScalePopInTime)).Subscribe(_ => { }, () =>
				{
					MovementLocked = false;
					_onTileRemoved?.OnNext(shufflePath);
				}).AddTo(this);
			}).AddTo(this);
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

			// combo removals require at least three of the same color in the matching path
			if (_controller.Tile.MatchingColorPatch == null || _controller.Tile.MatchingColorPatch.Count <= 2)
			{
				Debug.LogWarning("A combo requires at least three matching color tiles together");
				return;
			}

			MovementLocked = true;
			var shufflePath = _controller.Tile.MatchingColorPatch;

			for (int i = 0; i < shufflePath.Count; i++)
			{
				var index = i;
				var tileView = _tileLevelMap.GetTileView(shufflePath[i]);
				var sourceScale = tileView.transform.localScale;

				MainThreadDispatcher.StartUpdateMicroCoroutine(tileView.ScalePopInAnimation(Vector3.zero));
				Observable.Timer(TimeSpan.FromSeconds(_animationSettings.ScalePopInTime)).Subscribe(_ => { }, () =>
				{
					if (index == shufflePath.Count - 1)
					{
						tileView.Controller.RemoveCombo();
					}

					MainThreadDispatcher.StartUpdateMicroCoroutine(tileView.ScalePopInAnimation(sourceScale));
					Observable.Timer(TimeSpan.FromSeconds(_animationSettings.ScalePopInTime)).Subscribe(_ => { }, () =>
					{
						if (index == shufflePath.Count - 1)
						{
							MovementLocked = false;
							_onComboRemoval?.OnNext(shufflePath);
						}
					}).AddTo(this);
				}).AddTo(this);
			}
		}
	}
}