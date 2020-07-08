using System.Collections.Generic;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Persistence;
using TilesWalk.Map.General;
using TilesWalk.Map.Tile;
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

		private void Awake()
		{
			_solver.InstanceProvider(gameObject);
			CalculateAllGameStars();
		}

		public bool IsCompleted(LevelMap levelMap)
		{
			var count = GetHighestScoreStarCount(levelMap);
			return count == 3;
		}

		public LevelMapState State(LevelMap map)
		{
			var exists = _solver.Provider.Records.Exist(map.Id, out var score);
			var state = LevelMapState.None;

			if (!exists || score.Points.Highest < map.Target)
			{
				state = LevelMapState.ToComplete;
			}
			else if (score.Points.Highest > map.Target)
			{
				state = LevelMapState.Completed;
			}

			if (map.StarsRequired > GameStars)
			{
				state = LevelMapState.Locked;
			}

			return state;
		}

		public int GetStarCount(int target, int current)
		{
			var ratio = (float) current / target;

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

		public int GetHighestScoreStarCount(LevelMap levelMap)
		{
			var score = _solver.Provider.Records[levelMap.Id];
			return GetStarCount(levelMap.Target, score.Points.Highest);
		}

		public int GetLastScoreStarCount(LevelMap levelMap)
		{
			var score = _solver.Provider.Records[levelMap.Id];
			return GetStarCount(levelMap.Target, score.Points.Last);
		}

		public int GetHighestScoreStarCount(LevelScore score)
		{
			if (_solver.Provider.Collection.AvailableMaps == null ||
			    _solver.Provider.Collection.AvailableMaps.Count == 0)
			{
				Debug.LogWarning($"No maps found for this MapProviderSolver, {_solver.Source}");
				return 0;
			}

			var tileMap = _solver.Provider.Collection.AvailableMaps.Find(x => x.Id == score.Id);

			if (tileMap != null)
			{
				return GetStarCount(tileMap.Target, score.Points.Highest);
			}

			return 0;
		}

		public int GetLastScoreStarCount(LevelScore score)
		{
			var tileMap = _solver.Provider.Collection.AvailableMaps.Find(x => x.Id == score.Id);

			if (tileMap != null)
			{
				return GetStarCount(tileMap.Target, score.Points.Last);
			}

			return 0;
		}

		private void CalculateAllGameStars()
		{
			if (_solver.Provider.Records == null || _solver.Provider.Records.Count == 0) return;

			GameStars = 0;

			foreach (var scoreRecord in _solver.Provider.Records.Values)
			{
				GameStars += GetHighestScoreStarCount(scoreRecord);
			}
		}
	}
}