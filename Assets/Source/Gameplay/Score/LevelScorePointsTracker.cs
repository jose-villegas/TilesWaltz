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
		[Inject] private TileViewLevelMap _tileLevelMap;
		[Inject] private Dictionary<string, LevelScore> _scoreRecords;

		private Dictionary<string, int> _scoreTracking = new Dictionary<string, int>();

		protected Subject<LevelScore> _onScoreUpdated;
		protected Subject<LevelScore> _onScoresLoaded;

		public LevelScore LevelScore
		{
			get
			{
				if (!_scoreRecords.TryGetValue(_tileLevelMap.LevelMap.Id, out var score))
				{
					_scoreRecords[_tileLevelMap.LevelMap.Id] = new LevelScore(_tileLevelMap.LevelMap.Id);
				}

				return _scoreRecords[_tileLevelMap.LevelMap.Id];
			}
		}

		private void Start()
		{
			_tileLevelMap.OnTileRemovedAsObservable().Subscribe(OnTileRemoved).AddTo(this);
			_tileLevelMap.OnComboRemovalAsObservable().Subscribe(OnComboRemoval).AddTo(this);
			_tileLevelMap.OnLevelMapLoadedAsObservable().Subscribe(OnLevelMapLoaded).AddTo(this);
		}

		private void OnLevelMapLoaded(LevelMap _)
		{
			AddPoints(0);
			_onScoresLoaded?.OnNext(LevelScore);
		}

		public void AddPoints(int points)
		{
			var mapName = _tileLevelMap.LevelMap.Id;

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