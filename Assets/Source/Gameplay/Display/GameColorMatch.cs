using System;
using UnityEngine;

namespace TilesWalk.Gameplay.Display
{
	[Serializable]
	public class GameColorMatch
	{
		[SerializeField] private GameColor _name;
		[SerializeField] private TileColor _tile;
		[SerializeField] private Color _color;

		public GameColor Name => _name;

		public Color Color => _color;

		public TileColor Tile => _tile;
	}
}