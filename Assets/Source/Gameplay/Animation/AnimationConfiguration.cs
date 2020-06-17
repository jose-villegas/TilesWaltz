using System;
using UnityEngine;

namespace TilesWalk.Gameplay.Animation
{
	[Serializable]
	public class AnimationConfiguration
	{
		[SerializeField] [Min(0f)] private float _shuffleMoveTime;
		[SerializeField] [Min(0f)] private float _scalePopInTime;

		public float ShuffleMoveTime => _shuffleMoveTime;

		public float ScalePopInTime => _scalePopInTime;
	}
}