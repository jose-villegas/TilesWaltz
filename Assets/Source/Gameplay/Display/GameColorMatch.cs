using System;
using TilesWalk.Map.Tile;
using UnityEngine;

namespace TilesWalk.Gameplay.Display
{
	[Serializable]
	public class GameColorMatch
	{
		[SerializeField] private GameColor _name;
		[SerializeField] private Color _color;

		[Header("Gameplay Levels")] [SerializeField] [Space]
		private TileColor _tile;

		[Header("Map")] [SerializeField] private LevelMapState _levelMapState;


		public GameColor Name => _name;

		public Color Color => _color;

		public TileColor Tile => _tile;

		public LevelMapState LevelMapState => _levelMapState;
	}
}