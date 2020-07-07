using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Display;
using TilesWalk.General;
using TilesWalk.Map.Scaffolding;
using TilesWalk.Tile;
using TilesWalk.Tile.Rules;
using UniRx;
using UnityEngine;
using Zenject;
using LevelTileView = TilesWalk.Tile.Level.LevelTileView;

namespace TilesWalk.Map.Tile
{
	public class GameMapTile : TileView
	{
		[Inject] private GameMapTileFactory _factory;
		[Inject] private GameLevelsMapBuilder _map;
		[Inject] private DiContainer _container;

		[SerializeField] private string _levelId;
#if UNITY_EDITOR
		[SerializeField] private CardinalDirection _direction = CardinalDirection.North;
		[SerializeField] private NeighborWalkRule _rule = NeighborWalkRule.Plain;

		public string LevelId => _levelId;

		[Button(enabledMode: EButtonEnableMode.Editor)]
		private void AddNeighbor()
		{
			if (!_controller.Tile.IsValidInsertion(_direction, _rule))
			{
				Debug.LogError("Cannot insert a neighbor here, space already occupied ");
				return;
			}

			// since on editor mode dependencies cannot be solved, find the scripts
			if (_factory == null)
			{
				_factory = FindObjectOfType<GameMapTileFactory>();
			}

			// since on editor mode dependencies cannot be solved, find the scripts
			if (_map == null)
			{
				_map = FindObjectOfType<GameLevelsMapBuilder>();
			}

			var tile = _factory.NewInstance();
			this.InsertNeighbor(_direction, _rule, tile);

			// keep the same rule as parent, easier building
			tile._direction = _direction;
			tile._rule = _rule;
			// add new insertion instruction for this tile
			_map.RegisterTile(tile);
			_map.UpdateInstructions(this, tile, _direction, _rule);
		}
#endif

		[Button(enabledMode: EButtonEnableMode.Editor)]
		public void ConvertToLevelTile()
		{
			var hasLevelTile = GetComponentInChildren<GameLevelTile>();

			if (hasLevelTile)
			{
				Debug.LogWarning("This tile already has a GameLevelTile component");
				return;
			}

			if (Application.isEditor)
			{
				gameObject.AddComponent<GameLevelTile>();
				gameObject.AddComponent<GameLevelTileLinksHandler>();
				var requestHandler = gameObject.AddComponent<LevelNameRequestHandler>();
				requestHandler.RawName = _levelId;
			}
			else
			{
				_container.InstantiateComponent(typeof(GameLevelTile), gameObject);
				_container.InstantiateComponent(typeof(GameLevelTileLinksHandler), gameObject);
				var requestHandler =
					_container.InstantiateComponent(typeof(LevelNameRequestHandler), gameObject) as
						LevelNameRequestHandler;

				if (requestHandler != null)
				{
					requestHandler.Name.Value = _levelId;
				}
			}
		}

		protected override void UpdateColor(Tuple<TilesWalk.Tile.Tile, TileColor> color)
		{
		}

		protected override void OnGameResumed(Unit u)
		{
		}

		protected override void OnGamePaused(Unit u)
		{
		}
	}
}