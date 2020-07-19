using System;
using System.Collections;
using System.Collections.Generic;
using TilesWalk.Gameplay.Animation;
using TilesWalk.General;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace TilesWalk.Tile.Level
{
    public partial class LevelTileView
    {
        [Inject] protected AnimationConfiguration _animationSettings;

        private IEnumerator ScalePopInAnimation(Vector3 scale, float delay = 0f)
        {
            var t = 0f;
            var origin = transform.localScale;

            while (t <= delay)
            {
                t += Time.deltaTime;
                yield return null;
            }

            t = 0f;

            while (t <= _animationSettings.ScalePopInTime)
            {
                var norm = t / _animationSettings.ScalePopInTime;
                transform.localScale = Vector3.Lerp(origin, scale, _animationSettings.ScalePopInCurve.Evaluate(norm));
                t += Time.deltaTime;
                yield return null;
            }

            transform.localScale = scale;
        }

        private IEnumerator ShuffleMoveAnimation(List<Level.LevelTileView> tiles,
            List<Tuple<Vector3, Quaternion>> source)
        {
            var t = 0f;

            var backup = new List<Tuple<Vector3, Quaternion>>();

            for (var i = 0; i < tiles.Count && i < source.Count; i++)
            {
                backup.Add(new Tuple<Vector3, Quaternion>(tiles[i].transform.position, tiles[i].transform.rotation));
            }

            while (t <= _animationSettings.ShuffleMoveTime)
            {
                for (var i = 0; i < tiles.Count && i < source.Count && i < backup.Count; i++)
                {
                    var tile = tiles[i];
                    var norm = t / _animationSettings.ShuffleMoveTime;
                    tile.transform.position = Vector3.Lerp(backup[i].Item1, source[i].Item1,
                        _animationSettings.ShuffleCurve.Evaluate(norm));
                    tile.transform.rotation = Quaternion.Slerp(backup[i].Item2, source[i].Item2,
                        _animationSettings.ShuffleCurve.Evaluate(norm));
                }

                t += Time.deltaTime;
                yield return null;
            }

            for (var i = 0; i < tiles.Count && i < source.Count && i < backup.Count; i++)
            {
                var tile = tiles[i];
                tile.transform.position = source[i].Item1;
                tile.transform.rotation = source[i].Item2;
            }
        }
    }
}