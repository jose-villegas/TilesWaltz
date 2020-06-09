using System;
using System.Collections.Generic;
using TilesWalk.Gameplay.Score;
using TilesWalk.Gameplay.Score.Installer;
using TilesWalk.Navigation.Map;
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

		protected override void OnTileMapFound()
		{
			if (LevelName?.Value == null) return;

			if (!_scoreRecords.TryGetValue(LevelName.Value, out var levelScore))
			{
				return;
			}

			var ratio = (float) TileMap.Target / levelScore.Points.Highest;

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