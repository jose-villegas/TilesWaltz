using System;
using UnityEngine;

namespace TilesWalk.Gameplay.Animation
{
	[Serializable]
	public class AnimationConfiguration
	{
		[Header("Tiles")]
		[SerializeField] [Min(0f)] private float _shuffleMoveTime;
		[SerializeField] [Min(0f)] private float _scalePopInTime;

		[Header("Map Camera")] [SerializeField, Min(0f)]
		private float _targetTileTime;

		public float ShuffleMoveTime => _shuffleMoveTime;

		public float ScalePopInTime => _scalePopInTime;

		public float TargetTileTime => _targetTileTime;
	}
}