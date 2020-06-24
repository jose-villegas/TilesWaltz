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

		public LevelTileLink GetLink(CardinalDirection direction)
		{
			if (_links == null) _links = new List<LevelTileLink>();

			if (_links.Count > 0)
			{
				var indexOf = _links.FindIndex(x => x.Direction == direction);

				if (indexOf >= 0)
				{
					return _links[indexOf];
				}

				_links.Add(new LevelTileLink(direction));
				return _links[_links.Count - 1];
			}

			_links.Add(new LevelTileLink(direction));
			return _links[_links.Count - 1];
		}

		/// <summary>
		/// Finds the link that has a matching endpoint within its path,
		/// if it doesn't exist it will add a link in the given direction
		/// </summary>
		/// <param name="endpoint"></param>
		/// <param name="direction"></param>
		/// <returns></returns>
		public LevelTileLink GetLink(GameObject endpoint, CardinalDirection direction)
		{
			if (_links == null) _links = new List<LevelTileLink>();

			if (_links.Count > 0)
			{
				var indexOf = _links.FindIndex(x => x.Path[x.Path.Count - 1] == endpoint);

				if (indexOf >= 0)
				{
					return _links[indexOf];
				}

				_links.Add(new LevelTileLink(direction));
				return _links[_links.Count - 1];
			}

			_links.Add(new LevelTileLink(direction));
			return _links[_links.Count - 1];
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