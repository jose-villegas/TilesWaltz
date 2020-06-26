using System;
using UnityEngine;

namespace TilesWalk.Gameplay.Level
{
	[Serializable]
	public class CustomLevelsConfiguration
	{
		[SerializeField, Min(0)] private int _maximumUserMaps;
		[SerializeField, Min(0)] private int _maximumImportedMaps;
		[SerializeField, Min(0)] private int _maximumTilesPerLevel;
		[SerializeField, Range(0f, 1f)] private float _tileSeparationBoundsOffset;

		public int MaximumUserMaps => _maximumUserMaps;

		public int MaximumImportedMaps => _maximumImportedMaps;

		public int MaximumTilesPerLevel => _maximumTilesPerLevel;
		public float TileSeparationBoundsOffset => _tileSeparationBoundsOffset;
	}
}