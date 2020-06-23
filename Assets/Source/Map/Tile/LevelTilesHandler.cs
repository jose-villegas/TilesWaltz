using System;
using System.Collections.Generic;
using System.Linq;
using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Persistence;
using TilesWalk.Gameplay.Score;
using TilesWalk.Map.General;
using TilesWalk.Navigation.UI;
using UniRx;
using UniRx.Triggers;
using Zenject;

namespace TilesWalk.Map.Tile
{
	public class LevelTilesHandler : ObservableTriggerBase
	{
		[Inject] private LevelMapDetailsCanvas _detailsCanvas;
		[Inject] private MapProviderSolver _solver;

		private LevelTile[] _levelTiles;
		private ReactiveProperty<int> _readyCount = new ReactiveProperty<int>();

		private Subject<LevelTile[]> _levelTilesMapsReady;

		public LevelTile[] LevelTiles => _levelTiles;

		public LevelTile this[LevelMap map]
		{
			get
			{
				return _levelTiles.FirstOrDefault(x => x.Map.Value.Id == map.Id);
			}
		}

		public LevelTile this[int i]
		{
			get
			{
				return _levelTiles[i];
			}
		}

		private void Awake()
		{
			_solver.InstanceProvider(gameObject);

			var inChildren = GetComponentsInChildren<LevelTile>();
			_levelTiles = new LevelTile[inChildren.Length];

			for (int i = 0; i < inChildren.Length; i++)
			{
				var index = i;
				var levelTile = inChildren[index];
				levelTile.Map.Subscribe(tileMap =>
				{
					_levelTiles[index] = levelTile;
					_readyCount.Value += 1;
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
				if (_solver.Provider.Records.Exist(level.Name.Value, out var score))
				{
					if (score.Points.Highest < level.Map.Value.Target)
					{
						_detailsCanvas.LevelRequest.Name.Value = level.Map.Value.Id;
						_detailsCanvas.Show();
						return;
					}
				}
				else
				{
					_detailsCanvas.LevelRequest.Name.Value = level.Map.Value.Id;
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