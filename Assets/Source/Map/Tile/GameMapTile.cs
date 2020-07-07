using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Display;
using TilesWalk.General;
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

#if UNITY_EDITOR
		[SerializeField] private string _levelId;
		[SerializeField] private CardinalDirection _direction = CardinalDirection.North;
		[SerializeField] private NeighborWalkRule _rule = NeighborWalkRule.Plain;

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

		private void ConvertToLevelTile()
		{
			var hasLevelTile = GetComponentInChildren<GameLevelTile>();

			if (hasLevelTile)
			{
				Debug.LogWarning("This tile already has a GameLevelTile component");
				return;
			}

			// look for the mesh renderer
			var meshRenderer = GetComponentInChildren<MeshRenderer>();

			_container.InstantiateComponent(typeof(GameLevelTile), meshRenderer.gameObject);
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