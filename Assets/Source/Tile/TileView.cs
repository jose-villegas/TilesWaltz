using NaughtyAttributes;
using System;
using System.Collections.Generic;
using TilesWalk.BaseInterfaces;
using TilesWalk.Building;
using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Condition;
using TilesWalk.Gameplay.Display;
using TilesWalk.General;
using TilesWalk.Tile.Rules;
using UniRx;
using UniRx.Triggers;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace TilesWalk.Tile
{
	public partial class TileView : TileViewTrigger, IView
	{
		[SerializeField] private TileController _controller;
		[Inject] private TileViewFactory _tileFactory;
		[Inject] private TileViewLevelMap _tileLevelMap;
		[Inject] private LevelFinishTracker _levelFinishTracker;
		[Inject] private GameTileColorsConfiguration _tileColorsSettings;

		private MeshRenderer _meshRenderer;
		private BoxCollider _collider;

		public static bool MovementLocked { get; private set; }

		private BoxCollider Collider
		{
			get
			{
				if (_collider == null)
				{
					_collider = GetComponent<BoxCollider>();
				}

				return _collider;
			}
		}

		private MeshRenderer Renderer
		{
			get
			{
				if (_meshRenderer == null)
				{
					_meshRenderer = GetComponent<MeshRenderer>();
				}

				return _meshRenderer;
			}
		}

		public TileController Controller
		{
			get => _controller;
		}

		private static readonly Dictionary<TileColor, Material> Materials = new Dictionary<TileColor, Material>();

		public TileView()
		{
			_controller = new TileController();
		}

		private void OnDestroy()
		{
			if (Application.isEditor)
			{
				foreach (var item in _controller.Tile.Neighbors)
				{
					item.Value.Neighbors.Remove(item.Key.Opposite());
					item.Value.HingePoints.Remove(item.Key.Opposite());
				}

				_tileLevelMap.RefreshAllPaths();
				// unregistered when destroyed
				_tileLevelMap.RemoveTile(this);
			}

			MovementLocked = false;
		}

		private void Start()
		{
			if (Materials.Count == 0)
			{
				var colors = Enum.GetValues(typeof(TileColor));

				foreach (TileColor tileColor in colors)
				{
					Materials[tileColor] = new Material(Renderer.material) {color = _tileColorsSettings[tileColor] };
				}
			}

			// This small optimization enables us to share the material per color
			// instead of creating a new instance per every tile that tries to
			// change its color
			Renderer.material = Materials[_controller.Tile.TileColor];
			// update material on color update
			_controller.Tile.ObserveEveryValueChanged(x => x.TileColor).Subscribe(UpdateColor).AddTo(this);
			// check for combos
			transform.UpdateAsObservable().Subscribe(_ =>
			{
				if (_controller.Tile.MatchingColorPatch != null &&
				    _controller.Tile.MatchingColorPatch.Count > 2)
				{
					RemoveCombo();
				}
			}).AddTo(this);
			// on level finish stop interactions
			_levelFinishTracker.OnLevelFinishAsObservable().Subscribe(_ =>
			{
				MovementLocked = true;
				StartCoroutine(LevelFinishAnimation());
			}).AddTo(this);
		}

		private void OnMouseDown()
		{
			Remove();
		}

		private void UpdateColor(TileColor color)
		{
			Renderer.material = Materials[color];
		}

		#region Debug

#if UNITY_EDITOR
		[Header("Editor")] [SerializeField] private CardinalDirection direction = CardinalDirection.North;
		[SerializeField] private NeighborWalkRule rule = NeighborWalkRule.Plain;


		[Button]
		private void AddNeighbor()
		{
			if (!_controller.Tile.IsValidInsertion(direction, rule))
			{
				Debug.LogError("Cannot insert a neighbor here, space already occupied ");
				return;
			}

			var tile = _tileFactory.NewInstance();
			_controller.AddNeighbor(direction, rule, tile.Controller.Tile, transform, tile.transform);
			// keep the same rule as parent, easier building
			tile.direction = direction;
			tile.rule = rule;
			// add new insertion instruction for this tile
			_tileLevelMap.UpdateInstructions(this, tile, direction, rule);
		}

		private void OnDrawGizmos()
		{
			var tile = _controller.Tile;

			Gizmos.color = Color.blue;
			foreach (var value in tile.HingePoints.Values)
			{
				var translate = transform.position - tile.Index;
				Gizmos.DrawSphere(translate + value, 0.05f);
			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;

			if (_controller.Tile.ShortestPathToLeaf != null)
			{
				foreach (var tile in _controller.Tile.ShortestPathToLeaf)
				{
					var view = _tileLevelMap.GetTileView(tile);
					Gizmos.DrawCube(view.transform.position +
					                transform.up * 0.15f, Vector3.one * 0.15f);
				}
			}

			Gizmos.color = Color.magenta;
			if (_controller.Tile.MatchingColorPatch != null && _controller.Tile.MatchingColorPatch.Count > 2)
			{
				foreach (var tile in _controller.Tile.MatchingColorPatch)
				{
					var view = _tileLevelMap.GetTileView(tile);
					Gizmos.DrawWireCube(view.transform.position, Vector3.one);
				}
			}
		}
#endif

		#endregion
	}
}