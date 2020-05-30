using System;
using System.Collections.Generic;
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
		protected Subject<Score> _onScoreUpdated;

		private void Start()
		{
			_tileMap.OnTileRemovedAsObservable().Subscribe(OnTileRemoved).AddTo(this);
			_tileMap.OnComboRemovalAsObservable().Subscribe(OnComboRemoval).AddTo(this);
		}

		private void OnTileRemoved(List<Tile.Tile> tile)
		{
			var mapName = _tileMap.TileMap.Id;

			if (!_scoreRecords.TryGetValue(mapName, out var score))
			{
				_scoreRecords[mapName] = new Score(mapName);
				score = _scoreRecords[mapName];
			}

			score += _scoreConfiguration.ScorePerTile;
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

			score += _scoreConfiguration.ScorePerTile * _scoreConfiguration.ComboMultiplier * tile.Count;
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
	}
}