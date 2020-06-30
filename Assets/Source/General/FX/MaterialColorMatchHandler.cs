using System;
using System.Collections.Generic;
using TilesWalk.Gameplay.Display;
using TilesWalk.General.Patterns;
using TilesWalk.Map.Tile;
using UnityEngine;
using Zenject;

namespace TilesWalk.General.FX
{
	public abstract class MaterialColorMatchHandler<T1, T2> : MonoBehaviour
	{
		[Inject] protected GameTileColorsConfiguration _colorsConfiguration;

		[SerializeField] protected Material _sourceMaterial;

		protected static readonly Dictionary<T1, Material> Materials = new Dictionary<T1, Material>();

		protected virtual void Awake()
		{
			if (Materials.Count == 0)
			{
				var colors = Enum.GetValues(typeof(T1));

				foreach (T1 match in colors)
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

		public abstract Material GetMaterial(T2 match);
	}

	public class MaterialColorMatchHandler<T1> : MaterialColorMatchHandler<T1, T1>
	{
		public override Material GetMaterial(T1 match)
		{
			return Materials[match];
		}
	}
}