using System;
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

namespace TilesWalk.Map.Tile
{
	public class GameMapTile : TileView
	{
		[Inject] private GameMapTileFactory _factory;
		[Inject] private GameLevelsMapBuilder _map;
		[Inject] private DiContainer _container;

		[Header("Level Tile")]
		[SerializeField] private string _levelId;

		public string LevelId
		{
			get => _levelId;
			set => _levelId = value;
		}

#if UNITY_EDITOR
		[Header("Editor Mode")]
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

		[Button(enabledMode: EButtonEnableMode.Editor)]
		private void Remove()
		{
			// since on editor mode dependencies cannot be solved, find the scripts
			if (_map == null)
			{
				_map = FindObjectOfType<GameLevelsMapBuilder>();
			}

			_map.RemoveTile(this);

			foreach (var tileNeighbor in Controller.Tile.Neighbors)
			{
				tileNeighbor.Value.Neighbors.Remove(tileNeighbor.Key.Opposite());
			}

			DestroyImmediate(gameObject);
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
				// since on editor mode dependencies cannot be solved, find the scripts
				if (_map == null)
				{
					_map = FindObjectOfType<GameLevelsMapBuilder>();
				}

				// first the level name handler at root
				var requestHandler = gameObject.AddComponent<LevelNameRequestHandler>();
				requestHandler.RawName = _levelId;

				// then add interactive scripts to the model
				var meshRenderer = GetComponentInChildren<MeshRenderer>();
				meshRenderer.gameObject.AddComponent<GameLevelTile>();
				var linksHandler = meshRenderer.gameObject.AddComponent<GameLevelTileLinksHandler>();

				_map.RegisterLevelTile(this);
				linksHandler.ResolveLinks();

				// activate level ui canvas
				var canvas = GetComponentInChildren<Canvas>(true);
				canvas.gameObject.SetActive(true);
			}
			else
			{
				// first the level name handler at root
				var requestHandler =
					_container.InstantiateComponent(typeof(LevelNameRequestHandler), gameObject) as
						LevelNameRequestHandler;

				if (requestHandler != null)
				{
					requestHandler.Name.Value = _levelId;
				}

				// then add interactive scripts to the model
				var meshRenderer = GetComponentInChildren<MeshRenderer>();
				_container.InstantiateComponent(typeof(GameLevelTile), meshRenderer.gameObject);
				var linksHandler =
					_container.InstantiateComponent(typeof(GameLevelTileLinksHandler), meshRenderer.gameObject) as
						GameLevelTileLinksHandler;

				_map.RegisterLevelTile(this);

				if (linksHandler != null)
				{
					linksHandler.ResolveLinks();
				}

				// activate level ui canvas
				var canvas = GetComponentInChildren<Canvas>(true);
				canvas.gameObject.SetActive(true);
			}
		}

		protected override void Start()
		{
			_gameEvents.OnGamePausedAsObservable().Subscribe(OnGamePaused);
			_gameEvents.OnGameResumedAsObservable().Subscribe(OnGameResumed);

            var mapLayer = Animator.GetLayerIndex("Level");
            Animator.SetLayerWeight(mapLayer, 0f);
		}

		protected override void UpdateColor(Tuple<TilesWalk.Tile.Tile, TileColor> color)
		{
			// color setting will be handled by GameLevelsTilesInitializer
		}

		protected override void OnGameResumed(Unit u)
		{
		}

		protected override void OnGamePaused(Unit u)
		{
		}

        protected override void OnDestroy()
        {
        }
    }
}