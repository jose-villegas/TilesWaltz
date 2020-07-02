using System;
using System.Collections.Generic;
using TilesWalk.Gameplay.Display;
using TilesWalk.Tile;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Building.Level
{
	public class LevelMapEffectsHelper : MonoBehaviour
	{
		[Inject] private TileViewLevelMap _levelMap;

		[SerializeField] private ParticleSystem _colorMatch;
		[SerializeField] private ParticleSystem _verticalCut;
		[SerializeField] private ParticleSystem _horizontalCut;

		private void Awake()
		{
			_levelMap.OnPowerUpRemovalAsObservable().Subscribe(OnPowerUpRemoval).AddTo(this);
		}

		private void OnPowerUpRemoval(Tuple<List<Tile.Tile>, TilePowerUp> power)
		{
			switch (power.Item2)
			{
				case TilePowerUp.None:
					break;
				case TilePowerUp.NorthSouthLine:
					_verticalCut.Stop();
					_verticalCut.Play();
					break;
				case TilePowerUp.EastWestLine:
					_horizontalCut.Stop();
					_horizontalCut.Play();
					break;
				case TilePowerUp.ColorMatch:
					_colorMatch.Stop();
					_colorMatch.Play();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}