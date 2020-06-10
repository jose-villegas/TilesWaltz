using System.Collections.Generic;
using TilesWalk.Building.Level;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace TilesWalk.Gameplay.Score
{
	public class GameScoresHelper : MonoBehaviour
	{
		[Inject] private Dictionary<string, LevelScore> _scoreRecords;
		[Inject] private List<TileMap> _availableMaps;
		[Inject] private ScorePointsConfiguration _scorePointsSettings;

		public int GameStars { get; private set; }

		private void Start()
		{
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			CalculateAllGameStars();
		}

		public bool IsCompleted(TileMap tileMap)
		{
			var count = GetStarCount(tileMap);
			return count == 3;
		}

		public int GetStarCount(TileMap tileMap)
		{
			if (_scoreRecords.TryGetValue(tileMap.Id, out var score))
			{
				var ratio = (float) score.Points.Highest / tileMap.Target;

				if (ratio >= 1)
				{
					return 3;
				}

				if (ratio >= _scorePointsSettings.TwoStarRange)
				{
					return 2;
				}

				if (ratio >= _scorePointsSettings.OneStarRange)
				{
					return 1;
				}

				return 0;
			}

			return 0;
		}

		public int GetStarCount(LevelScore score)
		{
			var tileMap = _availableMaps.Find(x => x.Id == score.Id);

			if (tileMap != null)
			{
				var ratio = (float)score.Points.Highest / tileMap.Target;

				if (ratio >= 1)
				{
					return 3;
				}

				if (ratio >= _scorePointsSettings.TwoStarRange)
				{
					return 2;
				}

				if (ratio >= _scorePointsSettings.OneStarRange)
				{
					return 1;
				}

				return 0;
			}

			return 0;
		}

		private void CalculateAllGameStars()
		{
			if (_scoreRecords == null || _scoreRecords.Count == 0) return;

			GameStars = 0;

			foreach (var scoreRecord in _scoreRecords.Values)
			{
				GameStars += GetStarCount(scoreRecord);
			}
		}
	}
}