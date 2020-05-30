using System;
using System.Collections;
using System.Collections.Generic;
using TilesWalk.Gameplay;
using TilesWalk.Gameplay.Animation;
using UnityEngine;
using Zenject;

namespace TilesWalk.Tile
{
	public partial class TileView
	{
		[Inject] private AnimationConfiguration _animationSettings;

		private IEnumerator ScalePopInAnimation(Vector3 scale)
		{
			while ((scale - transform.localScale).sqrMagnitude > Mathf.Epsilon)
			{
				var step = _animationSettings.ScalePopInSpeed * Time.deltaTime;
				transform.localScale = Vector3.MoveTowards(transform.localScale, scale, step);
				yield return new WaitForEndOfFrame();
			}
		}

		private IEnumerator ShuffleMoveAnimation(List<TileView> tiles, List<Tuple<Vector3, Quaternion>> source)
		{
			var allTransformsDone = false;

			while (!allTransformsDone)
			{
				var step = _animationSettings.ShuffleMoveSpeed * Time.deltaTime;
				allTransformsDone = true;

				for (var i = 0; i < tiles.Count && i < source.Count; i++)
				{
					var tile = tiles[i];

					var offset = source[i].Item1 - tile.transform.position;

					allTransformsDone &= (source[i].Item1 - tile.transform.position).sqrMagnitude <= Mathf.Epsilon &&
					                     Quaternion.Angle(source[i].Item2, tile.transform.rotation) <= Mathf.Epsilon;

					tile.transform.position = Vector3.MoveTowards(tile.transform.position, source[i].Item1, step);
					tile.transform.rotation =
						Quaternion.RotateTowards(tile.transform.rotation, source[i].Item2,
							step * _animationSettings.ShuffleMoveAngularSpeed);
				}

				yield return new WaitForEndOfFrame();
			}
		}
	}
}