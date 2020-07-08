using System;
using System.Collections.Generic;
using TilesWalk.General;
using UnityEngine;

namespace TilesWalk.Map.Tile
{
	[Serializable]
	public class LevelTileLink
	{
		[SerializeField] private CardinalDirection _direction;
		[SerializeField] private GameLevelTile _level;
		[SerializeField] private List<GameObject> _path;

		public CardinalDirection Direction => _direction;

		public GameLevelTile Level
		{
			get => _level;
			set => _level = value;
		}

		public List<GameObject> Path
		{
			get => _path;
			set => _path = value;
		}

		public LevelTileLink(CardinalDirection direction)
		{
			_level = null;
			_direction = direction;
			_path = new List<GameObject>();
		}
	}
}