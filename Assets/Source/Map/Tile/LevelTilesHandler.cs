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
		private ReactiveProperty<int> _readyCount = new ReactiveProperty<int>();

		private Subject<LevelTile[]> _levelTilesMapsReady;

		public LevelTile[] LevelTiles { get; private set; }

		public LevelTile this[LevelMap map]
		{
			get
			{
				return LevelTiles.FirstOrDefault(x => x.Map.Value.Id == map.Id);
			}
		}

		public LevelTile this[int i] => LevelTiles[i];

		private void Start()
		{
			_solver.InstanceProvider(gameObject);

			var inChildren = GetComponentsInChildren<LevelTile>();
			LevelTiles = new LevelTile[inChildren.Length];

			for (int i = 0; i < inChildren.Length; i++)
			{
				var index = i;
				var levelTile = inChildren[index];
				levelTile.Map.Subscribe(tileMap =>
				{
					if (tileMap == null) return;

					LevelTiles[index] = levelTile;
					_readyCount.Value += 1;
				}).AddTo(this);
			}

			_readyCount.Subscribe(count =>
			{
				if (count == LevelTiles.Length)
				{
					_levelTilesMapsReady?.OnNext(LevelTiles);
					ShowNextLevelDetails();
				}
			}).AddTo(this);
		}

		private void ShowNextLevelDetails()
		{
			foreach (var level in LevelTiles)
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
			_detailsCanvas.LevelRequest.Name.Value = LevelTiles[0].Map.Value.Id;
			_detailsCanvas.Show();
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