using System;
using System.Collections.Generic;
using BayatGames.SaveGameFree;
using TilesWalk.Building.Map;
using UniRx;
using UniRx.Triggers;
using Zenject;

namespace TilesWalk.Gameplay.Score
{
	public class ScoreTracker : ObservableTriggerBase
	{
		[Inject] private ScoreConfiguration _scoreConfiguration;
		[Inject] private TileViewMap _tileMap;

		private Dictionary<int, Score> _scoreRecords = new Dictionary<int, Score>();
		private Dictionary<int, int> _scoreTracking = new Dictionary<int, int>();
		protected Subject<Score> _onScoreUpdated;
		protected Subject<Unit> _onScoresLoaded;

		public Score ActiveScore
		{
			get
			{
				if (!_scoreRecords.TryGetValue(_tileMap.TileMap.Id, out var score))
				{
					_scoreRecords[_tileMap.TileMap.Id] = new Score(_tileMap.TileMap.Id);
				}

				return _scoreRecords[_tileMap.TileMap.Id];
			}
		}

		private void Start()
		{
			_tileMap.OnTileRemovedAsObservable().Subscribe(OnTileRemoved).AddTo(this);
			_tileMap.OnComboRemovalAsObservable().Subscribe(OnComboRemoval).AddTo(this);
			_scoreRecords = SaveGame.Load("Scores", _scoreRecords);
			_onScoresLoaded?.OnCompleted();
		}

		private void OnDestroy()
		{
			SaveGame.Save("Scores", _scoreRecords);
		}

		private void OnTileRemoved(List<Tile.Tile> tile)
		{
			var mapName = _tileMap.TileMap.Id;

			if (!_scoreRecords.TryGetValue(mapName, out var score))
			{
				_scoreRecords[mapName] = new Score(mapName);
				score = _scoreRecords[mapName];
			}

			if (!_scoreTracking.TryGetValue(score.Id, out var track))
			{
				_scoreTracking[score.Id] = 0;
			}

			_scoreTracking[score.Id] += _scoreConfiguration.ScorePerTile;
			score.Update(_scoreTracking[score.Id]);
			_onScoreUpdated?.OnNext(score);
		}

		private void OnComboRemoval(List<Tile.Tile> tile)
		{
			var mapName = _tileMap.TileMap.Id;

			if (!_scoreRecords.TryGetValue(mapName, out var score))
			{
				_scoreRecords[mapName] = new Score(mapName);
				score = _scoreRecords[mapName];
			}

			if (!_scoreTracking.TryGetValue(score.Id, out var track))
			{
				_scoreTracking[score.Id] = 0;
			}

			_scoreTracking[score.Id] += _scoreConfiguration.ScorePerTile * _scoreConfiguration.ComboMultiplier * tile.Count;
			score.Update(_scoreTracking[score.Id]);
			_onScoreUpdated?.OnNext(score);
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			_onScoreUpdated?.OnCompleted();
			
		}

		public IObservable<Score> OnScoreUpdatedAsObservable()
		{
			return _onScoreUpdated = _onScoreUpdated ?? new Subject<Score>();
		}

		public IObservable<Unit> OnScoresLoadedAsObservable()
		{
			return _onScoresLoaded = _onScoresLoaded ?? new Subject<Unit>();
		}
	}
}