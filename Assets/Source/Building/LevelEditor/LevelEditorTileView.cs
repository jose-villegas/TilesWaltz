using System;
using System.Diagnostics;
using TilesWalk.Building.LevelEditor.UI;
using TilesWalk.Gameplay.Display;
using TilesWalk.General;
using TilesWalk.Tile;
using TilesWalk.Tile.Rules;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Building.LevelEditor
{
	public class LevelEditorTileView : TileView
	{
		[Inject] private LevelEditorToolSet _levelEditorToolSet;

		private LevelEditorTileView _ghostTileView;
		private Bounds _ghostTileBounds;

		private NeighborWalkRule _currentRule = NeighborWalkRule.Plain;
		private CardinalDirection _currentDirection = CardinalDirection.None;

		public BoolReactiveProperty IsSelected { get; } = new BoolReactiveProperty();
		public bool IsGhost { get; private set; } = false;

		protected override void Start()
		{
			base.Start();
			MovementLocked = true;
			Renderer.material = Materials[TileColor.None];

			_tileLevelMap.OnTileClickedAsObservable().Subscribe(OnAnyTileClicked).AddTo(this);
			IsSelected.Subscribe(OnTileSelected).AddTo(this);

			// subscribe to UI actions
			foreach (var value in Enum.GetValues(typeof(NeighborWalkRule)))
			{
				var enumValue = (NeighborWalkRule) value;
				var toggle = _levelEditorToolSet.GetToggle(enumValue);
				toggle.onValueChanged.AddListener(val =>
				{
					_currentRule = val ? enumValue : _currentRule;

					if (val && IsSelected.Value)
					{
						// remove previous ghost
						RemoveGhostNeighbor(_currentDirection);
						InsertGhostNeighbor(_currentDirection, _currentRule);
					}
				});
			}

			foreach (var value in Enum.GetValues(typeof(CardinalDirection)))
			{
				var enumValue = (CardinalDirection) value;

				if (enumValue == CardinalDirection.None) continue;
				;

				var button = _levelEditorToolSet.GetButton(enumValue);
				button.onClick.AddListener(() =>
				{
					if (!IsSelected.Value) return;

					// remove previous ghost
					RemoveGhostNeighbor(_currentDirection);
					_currentDirection = enumValue;
					InsertGhostNeighbor(_currentDirection, _currentRule);
				});
			}

			_levelEditorToolSet.Confirm.onClick.AddListener(() =>
			{
				if (IsSelected.Value)
				{
					_ghostTileView.IsGhost = false;
					_ghostTileView = null;
					_levelEditorToolSet.UpdateButtons(_controller.Tile);
				}
			});

			_levelEditorToolSet.Cancel.onClick.AddListener(() =>
			{
				if (IsSelected.Value)
				{
					RemoveGhostNeighbor(_currentDirection);
					Destroy(_ghostTileView);
					_ghostTileView = null;
					_levelEditorToolSet.UpdateButtons(_controller.Tile);
				}
			});
		}

		private void OnTileSelected(bool isSelected)
		{
			if (isSelected)
			{
				// update buttons interaction
				_levelEditorToolSet.UpdateButtons(_controller.Tile);

				// set outline for this tile
				var newMaterials = new[]
				{
					Renderer.materials[0],
					_levelEditorToolSet.OutlineMaterial
				};
				Renderer.materials = newMaterials;
			}
			else
			{
				if (Renderer.materials.Length > 1)
				{
					Renderer.materials = new[] {Renderer.materials[0]};
				}
			}
		}

		private void OnAnyTileClicked(Tile.Tile tile)
		{
			IsSelected.Value = tile == _controller.Tile;
		}

		protected override void OnMouseDown()
		{
			if (IsGhost) return;

			_onTileClicked?.OnNext(_controller.Tile);
		}

		private void DrawGhostNeighbor()
		{
		}

		private void InsertGhostNeighbor(CardinalDirection direction, NeighborWalkRule rule)
		{
			if (direction == CardinalDirection.None) return;

			if (_ghostTileView == null)
			{
				_ghostTileView = _tileFactory.NewInstance<LevelEditorTileView>();
				_ghostTileBounds = GetComponent<BoxCollider>().bounds;
				_ghostTileView.IsGhost = true;
			}
			else
			{
				_ghostTileView.transform.position = Vector3.zero;
				_ghostTileView.transform.rotation = Quaternion.identity;
				_ghostTileView.Controller = new TileController();
				_ghostTileView.Controller.AdjustBounds(_ghostTileBounds);
			}

			this.InsertNeighbor(direction, rule, _ghostTileView);
			_levelEditorToolSet.UpdateButtons(_controller.Tile);
		}

		private void RemoveGhostNeighbor(CardinalDirection direction)
		{
			if (_ghostTileView != null)
			{
				_ghostTileView.transform.position = Vector3.zero;
				_ghostTileView.transform.rotation = Quaternion.identity;
				_ghostTileView.Controller = new TileController();
				_ghostTileView.Controller.AdjustBounds(_ghostTileBounds);
			}

			if (direction == CardinalDirection.None) return;

			_controller.RemoveNeighbor(direction);
			_levelEditorToolSet.UpdateButtons(_controller.Tile);
		}
	}
}