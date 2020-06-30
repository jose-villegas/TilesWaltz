using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using TilesWalk.BaseInterfaces;
using TilesWalk.Building;
using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Condition;
using TilesWalk.Gameplay.Display;
using TilesWalk.General;
using TilesWalk.General.FX;
using TilesWalk.Tile.Rules;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace TilesWalk.Tile
{
	public partial class TileView : TileViewTrigger, IView
	{
		[SerializeField] protected TileController _controller;
		[Inject] protected TileViewFactory _tileFactory;
		[Inject] protected TileViewLevelMap _tileLevelMap;
		[Inject] protected GameTileColorsConfiguration _tileColorsSettings;
		[Inject] protected DiContainer _container;
		[Inject] protected TileColorMaterialColorMatchHandler _colorHandler;
		[Inject(Optional = true)] protected LevelFinishTracker _levelFinishTracker;

		private MeshRenderer _meshRenderer;
		private BoxCollider _collider;
		private ParticleSystemsCollector _particleSystems;

		public static bool MovementLocked { get; protected set; }

		public BoxCollider Collider
		{
			get
			{
				if (_collider == null)
				{
					_collider = GetComponentInChildren<BoxCollider>();
				}

				return _collider;
			}
		}

		protected MeshRenderer Renderer
		{
			get
			{
				if (_meshRenderer == null)
				{
					_meshRenderer = GetComponentInChildren<MeshRenderer>();
				}

				return _meshRenderer;
			}
		}

		public TileController Controller
		{
			get => _controller;
			set => _controller = value;
		}

		public TileView()
		{
			_controller = new TileController();
		}

		private void OnDestroy()
		{
			MovementLocked = false;
		}

		protected virtual void Start()
		{
			// Fetch FX particle system in children
			_particleSystems = gameObject.AddComponent<ParticleSystemsCollector>();
			// This small optimization enables us to share the material per color
			// instead of creating a new instance per every tile that tries to
			// change its color
			Renderer.material = _colorHandler.GetMaterial(_controller.Tile.TileColor);
			// update material on color update
			_controller.Tile.ObserveEveryValueChanged(x => x.TileColor).Subscribe(UpdateColor).AddTo(this);
			// check for combos
			transform.UpdateAsObservable().Subscribe(_ =>
			{
				if (MovementLocked) return;

				if (_controller.Tile.MatchingColorPatch != null &&
				    _controller.Tile.MatchingColorPatch.Count > 2)
				{
					RemoveCombo();
				}
			}).AddTo(this);

			// on level finish stop interactions
			if (_levelFinishTracker != null)
			{
				_levelFinishTracker.OnLevelFinishAsObservable().Subscribe(_ =>
				{
					MovementLocked = true;
					MainThreadDispatcher.StartEndOfFrameMicroCoroutine(LevelFinishAnimation());
				}).AddTo(this);
			}
		}

		protected virtual void OnMouseDown()
		{
			_onTileClicked?.OnNext(_controller.Tile);
			Remove();
		}

		protected virtual void UpdateColor(TileColor color)
		{
			Renderer.material = _colorHandler.GetMaterial(color);
		}

		#region Debug

#if UNITY_EDITOR
		[Header("Editor")] private CardinalDirection direction = CardinalDirection.North;
		private NeighborWalkRule rule = NeighborWalkRule.Plain;


		[Button]
		private void AddNeighbor()
		{
			if (!_controller.Tile.IsValidInsertion(direction, rule))
			{
				Debug.LogError("Cannot insert a neighbor here, space already occupied ");
				return;
			}

			var tile = _tileFactory.NewInstance();
			this.InsertNeighbor(direction, rule, tile);

			// keep the same rule as parent, easier building
			tile.direction = direction;
			tile.rule = rule;
			// add new insertion instruction for this tile
			_tileLevelMap.UpdateInstructions(this, tile, direction, rule);
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;

			if (_controller.Tile.ShortestPathToLeaf != null)
			{
				foreach (var tile in _controller.Tile.ShortestPathToLeaf)
				{
					if (!_tileLevelMap.HasTileView(tile)) continue;

					var view = _tileLevelMap.GetTileView(tile);
					Gizmos.DrawCube(view.transform.position +
					                view.transform.up * 0.15f, Vector3.one * 0.15f);
				}
			}

			Gizmos.color = Color.blue;
			foreach (var hingePoint in _controller.Tile.HingePoints)
			{
				var relative = transform.rotation * hingePoint.Value;
				Gizmos.DrawSphere(transform.position + relative, 0.05f);

				if (!_tileLevelMap.HasTileView(_controller.Tile.Neighbors[hingePoint.Key])) continue;

				var view = _tileLevelMap.GetTileView(_controller.Tile.Neighbors[hingePoint.Key]);
				var joint = view.transform.rotation * view.Controller.Tile.HingePoints[hingePoint.Key.Opposite()];
				Gizmos.DrawLine(transform.position + relative, view.transform.position + joint);
			}

			Gizmos.color = Color.magenta;
			if (_controller.Tile.MatchingColorPatch != null && _controller.Tile.MatchingColorPatch.Count > 2)
			{
				foreach (var tile in _controller.Tile.MatchingColorPatch)
				{
					if (!_tileLevelMap.HasTileView(tile)) continue;

					var view = _tileLevelMap.GetTileView(tile);
					Gizmos.DrawWireCube(view.transform.position, Vector3.one);
				}
			}
		}
#endif

		#endregion
	}
}