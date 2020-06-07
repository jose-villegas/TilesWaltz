using System;
using System.Collections.Generic;
using TilesWalk.Building.Level;
using UniRx;
using UniRx.Triggers;
using Zenject;

namespace TilesWalk.Gameplay.Score
{
	public class LevelScorePointsTracker : ObservableTriggerBase
	{
		[Inject] private ScorePointsConfiguration _scorePointsConfiguration;
		[Inject] private TileViewMap _tileMap;
		[Inject] private Dictionary<string, LevelScore> _scoreRecords;

		private Dictionary<string, int> _scoreTracking = new Dictionary<string, int>();

		protected Subject<LevelScore> _onScoreUpdated;
		protected Subject<LevelScore> _onScoresLoaded;

		public LevelScore LevelScore
		{
			get
			{
				if (!_scoreRecords.TryGetValue(_tileMap.TileMap.Id, out var score))
				{
					_scoreRecords[_tileMap.TileMap.Id] = new LevelScore(_tileMap.TileMap.Id);
				}

				return _scoreRecords[_tileMap.TileMap.Id];
			}
		}

		private void Start()
		{
			_tileMap.OnTileRemovedAsObservable().Subscribe(OnTileRemoved).AddTo(this);
			_tileMap.OnComboRemovalAsObservable().Subscribe(OnComboRemoval).AddTo(this);
			_tileMap.OnTileMapLoadedAsObservable().Subscribe(_ => { _onScoresLoaded?.OnNext(LevelScore); }).AddTo(this);
		}

		public void AddPoints(int points)
		{
			var mapName = _tileMap.TileMap.Id;

			if (!_scoreRecords.TryGetValue(mapName, out var score))
			{
				_scoreRecords[mapName] = new LevelScore(mapName);
				score = _scoreRecords[mapName];
			}

			if (!_scoreTracking.TryGetValue(score.Id, out var track))
			{
				_scoreTracking[score.Id] = 0;
			}

			_scoreTracking[score.Id] += points;
			score.Points.Update(_scoreTracking[score.Id]);
			_onScoreUpdated?.OnNext(score);
		}

		private void OnTileRemoved(List<Tile.Tile> tile)
		{
			AddPoints(_scorePointsConfiguration.PointsPerTile);
		}

		private void OnComboRemoval(List<Tile.Tile> tile)
		{
			AddPoints(_scorePointsConfiguration.PointsPerTile *
			          _scorePointsConfiguration.ComboMultiplier * tile.Count);
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			_onScoreUpdated?.OnCompleted();
			_onScoresLoaded?.OnCompleted();
		}

		public IObservable<LevelScore> OnScorePointsUpdatedAsObservable()
		{
			return _onScoreUpdated = _onScoreUpdated ?? new Subject<LevelScore>();
		}

		public IObservable<LevelScore> OnScoresLoadedAsObservable()
		{
			return _onScoresLoaded = _onScoresLoaded ?? new Subject<LevelScore>();
		}
	}
}