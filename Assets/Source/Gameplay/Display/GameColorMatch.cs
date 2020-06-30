using System;
using TilesWalk.Map.Tile;
using UnityEngine;

namespace TilesWalk.Gameplay.Display
{
	[Serializable]
	public class GameColorMatch<T>
	{
		[SerializeField] private Color _color;
		[SerializeField] private T _match;

		public T Match => _match;
		public Color Color => _color;
	}
}