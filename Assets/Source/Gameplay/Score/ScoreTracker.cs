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
		[Inject] private Dictionary<string, Score> _scoreRecords;

		private Dictionary<string, int> _scoreTracking = new Dictionary<string, int>();

		protected Subject<Score> _onScoreUpdated;
		protected Subject<Score> _onScoresLoaded;

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
			_tileMap.OnTileMapLoadedAsObservable().Subscribe(_ => { _onScoresLoaded?.OnNext(ActiveScore); }).AddTo(this);
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
			score.UpdateScore(_scoreTracking[score.Id]);
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

			_scoreTracking[score.Id] +=
				_scoreConfiguration.ScorePerTile * _scoreConfiguration.ComboMultiplier * tile.Count;
			score.UpdateScore(_scoreTracking[score.Id]);
			_onScoreUpdated?.OnNext(score);
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			_onScoreUpdated?.OnCompleted();
			_onScoresLoaded?.OnCompleted();
		}

		public IObservable<Score> OnScoreUpdatedAsObservable()
		{
			return _onScoreUpdated = _onScoreUpdated ?? new Subject<Score>();
		}

		public IObservable<Score> OnScoresLoadedAsObservable()
		{
			return _onScoresLoaded = _onScoresLoaded ?? new Subject<Score>();
		}
	}
}