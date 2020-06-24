using System;
using System.Collections.Generic;
using System.Linq;
using TilesWalk.Building.Level;
using TilesWalk.General;
using TilesWalk.Map.Bridge;
using TilesWalk.Map.Scaffolding;
using TilesWalk.Navigation.UI;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace TilesWalk.Map.Tile
{
	public class LevelTile : ObservableTriggerBase, ILevelNameRequire
	{
		[Serializable]
		private class LevelTileLink
		{
			[SerializeField] private CardinalDirection _direction;
			[SerializeField] private LevelTile _level;

			public CardinalDirection Direction => _direction;

			public LevelTile Level => _level;
		}

		[Inject] private LevelMapDetailsCanvas _detailsCanvas;
		[Inject] private LevelBridge _levelBridge;

		[SerializeField] private List<LevelTileLink> _links;

		public ReactiveProperty<string> Name { get; } = new ReactiveProperty<string>();
		public ReactiveProperty<LevelMap> Map { get; } = new ReactiveProperty<LevelMap>();

		private Subject<LevelTile> _onLevelTileClick;

		public LevelTile this[CardinalDirection direction]
		{
			get
			{
				return _links.Find(x => x.Direction == direction).Level;
			}
		}

		public bool HasNeighbor(CardinalDirection direction)
		{
			return _links.Any(x => x.Direction == direction);
		}

		private void OnMouseDown()
		{
			OnMapTileClick();
		}

		public void OnMapTileClick()
		{
			if (_detailsCanvas.IsVisible && _detailsCanvas.LevelRequest.Name.Value == Name.Value)
			{
				_detailsCanvas.Hide();
			}
			else
			{
				_detailsCanvas.LevelRequest.Name.Value = Name.Value;
				_detailsCanvas.Show();
			}

			_onLevelTileClick?.OnNext(this);
		}

		public IObservable<LevelTile> OnLevelTileClickAsObservable()
		{
			return _onLevelTileClick = _onLevelTileClick ?? new Subject<LevelTile>();
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			_onLevelTileClick?.OnCompleted();
		}
	}
}