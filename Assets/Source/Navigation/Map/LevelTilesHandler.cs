using System;
using System.Collections.Generic;
using TilesWalk.Gameplay.Score;
using TilesWalk.Navigation.UI;
using UniRx;
using UniRx.Triggers;
using Zenject;

namespace TilesWalk.Navigation.Map
{
	public class LevelTilesHandler : ObservableTriggerBase
	{
		[Inject] private TileMapDetailsCanvas _detailsCanvas;
		[Inject] private Dictionary<string, LevelScore> _scoreRecords;

		private LevelTile[] _levelTiles;
		private ReactiveProperty<int> _readyCount = new ReactiveProperty<int>();

		private Subject<LevelTile[]> _levelTilesMapsReady;

		private void Awake()
		{
			var inChildren = GetComponentsInChildren<LevelTile>();
			_levelTiles = new LevelTile[inChildren.Length];

			for (int i = 0; i < inChildren.Length; i++)
			{
				var index = i;
				var levelTile = inChildren[index];
				levelTile.TileMap.Subscribe(tileMap =>
				{
					if (tileMap != null)
					{
						_levelTiles[index] = levelTile;
						_readyCount.Value += 1;
					}
				}).AddTo(this);
			}

			_readyCount.Subscribe(count =>
			{
				if (count == _levelTiles.Length)
				{
					_levelTilesMapsReady?.OnNext(_levelTiles);
					ShowNextLevelDetails();
				}
			}).AddTo(this);
		}

		private void ShowNextLevelDetails()
		{
			foreach (var level in _levelTiles)
			{
				if (_scoreRecords.TryGetValue(level.TileMap.Value.Id, out var score))
				{
					if (score.Points.Highest < level.TileMap.Value.Target)
					{
						_detailsCanvas.LevelName.Value = level.TileMap.Value.Id;
						_detailsCanvas.Show();
						return;
					}
				}
				else
				{
					_detailsCanvas.LevelName.Value = level.TileMap.Value.Id;
					_detailsCanvas.Show();
					return;
				}
			}

			// no next map found
			_detailsCanvas.Hide();
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			_levelTilesMapsReady?.OnCompleted();
		}

		public IObservable<LevelTile[]> OnLevelTilesMapsReadyAsObservable()
		{
			return _levelTilesMapsReady = _levelTilesMapsReady ?? new Subject<LevelTile[]>();
		}
	}
}