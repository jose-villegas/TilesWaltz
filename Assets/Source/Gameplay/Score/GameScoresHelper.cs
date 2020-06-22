using System.Collections.Generic;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Persistence;
using TilesWalk.Map.General;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace TilesWalk.Gameplay.Score
{
	[RequireComponent(typeof(IMapProvider))]
	public class GameScoresHelper : MonoBehaviour
	{
		[Inject] private ScorePointsConfiguration _scorePointsSettings;
		[Inject] private MapProviderSolver _solver;

		public int GameStars { get; private set; }

		private void Start()
		{
			_solver.InstanceProvider(gameObject);

			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			CalculateAllGameStars();
		}

		public bool IsCompleted(LevelMap levelMap)
		{
			var count = GetStarCount(levelMap);
			return count == 3;
		}

		public int GetStarCount(LevelMap levelMap)
		{
			if (_solver.Provider.Records.TryGetValue(levelMap.Id, out var score))
			{
				var ratio = (float) score.Points.Highest / levelMap.Target;

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

		public int GetStarCount(LevelMap levelMap, int current)
		{
			var ratio = (float) current / levelMap.Target;

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

		public int GetStarCount(LevelScore score)
		{
			var tileMap = _solver.Provider.Collection.AvailableMaps.Find(x => x.Id == score.Id);

			if (tileMap != null)
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

		private void CalculateAllGameStars()
		{
			if (_solver.Provider.Records == null || _solver.Provider.Records.Count == 0) return;

			GameStars = 0;

			foreach (var scoreRecord in _solver.Provider.Records.Values)
			{
				GameStars += GetStarCount(scoreRecord);
			}
		}
	}
}