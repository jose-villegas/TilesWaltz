using System;
using System.Collections.Generic;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Display;
using TilesWalk.Gameplay.Score;
using TilesWalk.General.Patterns;
using TilesWalk.Map.Scaffolding;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Map.Tile
{
	[RequireComponent(typeof(MeshRenderer))]
	public class LevelTileMaterialHandler : ObligatoryComponentBehaviour<MeshRenderer>, ILevelNameRequire
	{
		[Inject] private GameScoresHelper _gameScoresHelper;
		[Inject] private GameTileColorsConfiguration _colorsConfiguration;

		public ReactiveProperty<string> Name { get; } = new ReactiveProperty<string>();
		public ReactiveProperty<LevelMap> Map { get; } = new ReactiveProperty<LevelMap>();

		private protected static readonly Dictionary<LevelMapState, Material> Materials =
			new Dictionary<LevelMapState, Material>();

		private void Start()
		{
			if (Materials.Count == 0)
			{
				var colors = Enum.GetValues(typeof(LevelMapState));

				foreach (LevelMapState tileColor in colors)
				{
					Materials[tileColor] = new Material(Component.material) {color = _colorsConfiguration[tileColor]};
				}
			}

			Map.Subscribe(map =>
			{
				if (map != null)
				{
					if (_gameScoresHelper.IsCompleted(map))
					{
						Component.material = Materials[LevelMapState.Completed];
					}
					else
					{
						if (_gameScoresHelper.GameStars < map.StarsRequired)
						{
							Component.material = Materials[LevelMapState.Locked];
						}
						else
						{
							Component.material = Materials[LevelMapState.ToComplete];
						}
					}
				}
			});
		}
	}
}