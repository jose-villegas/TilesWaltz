using System;
using UnityEngine;

namespace TilesWalk.Gameplay.Score
{
	[Serializable]
	public class ScoreConfiguration
	{
		[Range(1, 100)] [SerializeField] private int _scorePerTile = 10;
		[Range(1, 100)] [SerializeField] private int _comboMultiplier = 10;

		public int ScorePerTile => _scorePerTile;
		public int ComboMultiplier => _comboMultiplier;
	}
}