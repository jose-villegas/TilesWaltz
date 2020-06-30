using System;
using System.Collections.Generic;
using TilesWalk.Gameplay.Display;
using TilesWalk.General.Patterns;
using TilesWalk.Map.Tile;
using UnityEngine;
using Zenject;

namespace TilesWalk.General.FX
{
	public class MaterialColorMatchHandler<T> : MonoBehaviour
	{
		[Inject] private GameTileColorsConfiguration _colorsConfiguration;

		[SerializeField] private Material _sourceMaterial;

		private static readonly Dictionary<T, Material> Materials = new Dictionary<T, Material>();

		private void Awake()
		{
			if (Materials.Count == 0)
			{
				var colors = Enum.GetValues(typeof(T));

				foreach (T match in colors)
				{
					if (match is TileColor tileColor)
					{
						Materials[match] = new Material(_sourceMaterial)
							{color = _colorsConfiguration[tileColor]};
					}

					if (match is GameColor color)
					{
						Materials[match] = new Material(_sourceMaterial)
							{color = _colorsConfiguration[color]};
					}

					if (match is LevelMapState state)
					{
						Materials[match] = new Material(_sourceMaterial)
							{color = _colorsConfiguration[state]};
					}
				}
			}
		}

		public Material GetMaterial(T match)
		{
			return Materials[match];
		}
	}
}