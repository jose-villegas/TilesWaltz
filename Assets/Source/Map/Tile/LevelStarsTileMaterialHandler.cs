using TilesWalk.General.FX;
using UnityEngine;

namespace TilesWalk.Map.Tile
{
	public class LevelStarsTileMaterialHandler : MaterialColorMatchHandler<Color, int>
	{
		protected override void Awake()
		{
		}

		public override Material GetMaterial(int match)
		{
			var found = _colorsConfiguration.GetMapStarsColor(match);

			if (!Materials.TryGetValue(found, out var material))
			{
				material = new Material(_sourceMaterial) {color = found};
			}

			return material;
		}
	}
}