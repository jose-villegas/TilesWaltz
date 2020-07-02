using System;
using UnityEngine;

namespace TilesWalk.Gameplay.Animation
{
	[Serializable]
	public class AnimationConfiguration
	{
		[Header("Tiles")] [SerializeField] private AnimationCurve _shuffleCurve;
		[SerializeField] [Min(0f)] private float _shuffleMoveTime;
		[SerializeField] private AnimationCurve _scalePopInCurve;
		[SerializeField] [Min(0f)] private float _scalePopInTime;
		[SerializeField] private float _powerUpPopInSequenceFactor;

		[Header("Map Camera")] [SerializeField, Min(0f)]
		private float _targetTileTime;

		public float ShuffleMoveTime => _shuffleMoveTime;

		public float ScalePopInTime => _scalePopInTime;

		public float TargetTileTime => _targetTileTime;

		public AnimationCurve ShuffleCurve => _shuffleCurve;

		public AnimationCurve ScalePopInCurve => _scalePopInCurve;

		public float PowerUpPopInSequenceFactor => _powerUpPopInSequenceFactor;
	}
}