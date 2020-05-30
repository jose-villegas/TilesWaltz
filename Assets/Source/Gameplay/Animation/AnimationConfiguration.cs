using System;
using UnityEngine;

namespace TilesWalk.Gameplay.Animation
{
	[Serializable]
	public class AnimationConfiguration
	{
		[SerializeField] [Min(1f)] private float _shuffleMoveSpeed;
		[SerializeField] [Min(1f)] private float _shuffleMoveAngularSpeed;
		[SerializeField] [Min(1f)] private int _scalePopInSpeed;

		public float ShuffleMoveSpeed => _shuffleMoveSpeed;

		public int ScalePopInSpeed => _scalePopInSpeed;

		public float ShuffleMoveAngularSpeed => _shuffleMoveAngularSpeed;
	}
}