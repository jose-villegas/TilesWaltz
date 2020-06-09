using System;
using UnityEngine;

namespace TilesWalk.Gameplay.Score
{
	[Serializable]
	public class ScorePointsConfiguration
	{
		[Range(1, 100)] [SerializeField] private int _pointsPerTile = 10;
		[Range(1, 100)] [SerializeField] private int _comboMultiplier = 10;
		[Range(1, 100)] [SerializeField] private int _pointsPerExtraSecond = 20;
		[Range(1, 1000)] [SerializeField] private int _pointsPerExtraMove = 100;

		[Range(0f, 1f)] [SerializeField] private float _oneStarRange = 0.25f;
		[Range(0f, 1f)] [SerializeField] private float _twoStarRange = 0.65f;

		public int PointsPerTile => _pointsPerTile;
		public int ComboMultiplier => _comboMultiplier;

		public int PointsPerExtraSecond => _pointsPerExtraSecond;

		public int PointsPerExtraMove => _pointsPerExtraMove;

		public float OneStarRange => _oneStarRange;

		public float TwoStarRange => _twoStarRange;
	}
}