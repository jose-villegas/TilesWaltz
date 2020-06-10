using System;
using System.Collections.Generic;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Score;
using TilesWalk.Gameplay.Score.Installer;
using TilesWalk.Map.Scaffolding;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TilesWalk.Navigation.UI
{
	public class LevelTileStarsHandler : LevelNameRequireBehaviour
	{
		[SerializeField] private Sprite _starSprite;
		[SerializeField] private Sprite _starFilledSprite;
		[SerializeField] private Image[] _stars;

		[Inject] private Dictionary<string, LevelScore> _scoreRecords;
		[Inject] private ScorePointsConfiguration _scorePointsSettings;

		private void Awake()
		{
			OnTileMapFoundAsObservable().Subscribe(OnTileMapUpdated).AddTo(this);
		}

		protected void OnTileMapUpdated(TileMap tileMap)
		{
			if (tileMap == null) return;

			if (!_scoreRecords.TryGetValue(tileMap.Id, out var levelScore))
			{
				return;
			}

			var ratio = (float) levelScore.Points.Highest / tileMap.Target;

			for (int i = 0; i < _stars.Length; i++)
			{
				_stars[i].sprite = _starSprite;

				if (i == 0 && ratio > _scorePointsSettings.OneStarRange)
				{
					_stars[i].sprite = _starFilledSprite;
				}

				if (i == 1 && ratio > _scorePointsSettings.TwoStarRange)
				{
					_stars[i].sprite = _starFilledSprite;
				}

				if (i == 2 && ratio >= 1f)
				{
					_stars[i].sprite = _starFilledSprite;
				}
			}
		}
	}
}