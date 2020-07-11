using System;
using UnityEngine;

namespace TilesWalk.General.Display
{
    [Serializable]
    public class DisplayOption
    {
        [SerializeField] private Vector2 _aspectRatio;
        [SerializeField] private int _maxOrthogonalSize;

        public Vector2 Ratio => _aspectRatio;

        public int MaxOrthogonalSize => _maxOrthogonalSize;

        public float Aspect => _aspectRatio.x / _aspectRatio.y;
    }
}