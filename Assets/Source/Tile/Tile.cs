using TilesWalk.BaseInterfaces;
using UnityEngine;

namespace TilesWalk.Tile
{
	public class Tile : IModel
	{
		[SerializeField]
		private TileOrientation _orientation;

		[SerializeField]
		private Color _color;
		
		[SerializeField]
		private Tile[] neighbors;
	}
}

