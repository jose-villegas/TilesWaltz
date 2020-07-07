using System;
using System.Collections.Generic;
using TilesWalk.Gameplay.Display;
using TilesWalk.General.FX;
using TilesWalk.Tile;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Building.Level
{
	public class LevelMapEffectsHelper : MonoBehaviour
	{
		[Inject] private TileViewLevelMap __tileLevelMap;
		[Inject] protected GameTileColorsConfiguration _colorsConfiguration;

		[SerializeField] private ParticleSystemsCollector _particleSystems;

		private ParticleSystemRenderer _colorMatchRenderer;

		private void Awake()
		{
			__tileLevelMap.Trigger.OnPowerUpRemovalAsObservable().Subscribe(OnPowerUpRemoval).AddTo(this);
		}

		private void OnPowerUpRemoval(Tuple<List<Tile.Tile>, TilePowerUp> power)
		{
			switch (power.Item2)
			{
				case TilePowerUp.None:
					break;
				case TilePowerUp.NorthSouthLine:
					_particleSystems["CutVertical"].Stop();
					_particleSystems["CutVertical"].Play();
					break;
				case TilePowerUp.EastWestLine:
					_particleSystems["CutHorizontal"].Stop();
					_particleSystems["CutHorizontal"].Play();
					break;
				case TilePowerUp.ColorMatch:
					var color = _colorsConfiguration[power.Item1[0].TileColor];
					ParticleSystem.MainModule settings = _particleSystems["ColorPower"].main;
					settings.startColor = color;

					_particleSystems["ColorPower"].Stop();
					_particleSystems["ColorPower"].Play();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}