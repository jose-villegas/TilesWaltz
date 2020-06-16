using System;
using System.Diagnostics;
using System.Linq;
using TilesWalk.Building.Level;
using TilesWalk.Building.LevelEditor.UI;
using TilesWalk.Extensions;
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
		[Inject] private CustomLevelPlayer _customLevelPlayer;

		private LevelEditorTileView _ghostTileView;
		private Bounds _ghostTileBounds = new Bounds(Vector3.zero, new Vector3(1f, 0.3f, 1f));

		private NeighborWalkRule _currentRule = NeighborWalkRule.Plain;
		private CardinalDirection _currentDirection = CardinalDirection.None;

		public BoolReactiveProperty IsSelected { get; } = new BoolReactiveProperty();
		public bool IsGhost { get; private set; } = false;

		protected override void Start()
		{
			if (IsGhost) return;

			base.Start();
			MovementLocked = true;
			Renderer.material = IsGhost ? _levelEditorToolSet.GhostMaterial : Materials[TileColor.None];

			_tileLevelMap.OnTileClickedAsObservable().Subscribe(OnAnyTileClicked).AddTo(this);
			IsSelected.Subscribe(OnTileSelected).AddTo(this);

			_customLevelPlayer.OnPlayAsObservable().Subscribe(OnCustomLevelPlay).AddTo(this);
			_customLevelPlayer.OnStopAsObservable().Subscribe(OnCustomLevelStop).AddTo(this);

			_levelEditorToolSet.Confirm.interactable = false;
			_levelEditorToolSet.Cancel.interactable = false;
			_levelEditorToolSet.Delete.interactable = false;

			_levelEditorToolSet.Confirm.OnClickAsObservable().Subscribe(_ => OnConfirmClick()).AddTo(this);
			_levelEditorToolSet.Cancel.OnClickAsObservable().Subscribe(_ => OnCancelClick()).AddTo(this);
			_levelEditorToolSet.Delete.OnClickAsObservable().Subscribe(_ => OnDeleteClick()).AddTo(this);

			// subscribe to UI actions
			foreach (var value in Enum.GetValues(typeof(NeighborWalkRule)))
			{
				var enumValue = (NeighborWalkRule) value;
				var toggle = _levelEditorToolSet.GetToggle(enumValue);
				toggle.OnValueChangedAsObservable().Subscribe(val =>
				{
					_currentRule = val ? enumValue : _currentRule;

					if (val && IsSelected.Value)
					{
						// remove previous ghost
						RemoveGhostNeighbor(_currentDirection);
						InsertGhostNeighbor(_currentDirection, _currentRule);

						_levelEditorToolSet.Confirm.interactable = true;
						_levelEditorToolSet.Cancel.interactable = true;
					}
				}).AddTo(this);
			}

			foreach (var value in Enum.GetValues(typeof(CardinalDirection)))
			{
				var enumValue = (CardinalDirection) value;

				if (enumValue == CardinalDirection.None) continue;

				var button = _levelEditorToolSet.GetButton(enumValue);
				button.OnClickAsObservable().Subscribe(_ =>
				{
					if (!IsSelected.Value) return;

					// remove previous ghost
					RemoveGhostNeighbor(_currentDirection);
					_currentDirection = enumValue;
					InsertGhostNeighbor(_currentDirection, _currentRule);

					_levelEditorToolSet.Confirm.interactable = true;
					_levelEditorToolSet.Cancel.interactable = true;
				}).AddTo(this);
			}
		}

		private void OnCustomLevelStop(LevelMap obj)
		{
			MovementLocked = true;
			// return blank material
			Renderer.material = IsGhost ? _levelEditorToolSet.GhostMaterial : Materials[TileColor.None];
		}

		private void OnCustomLevelPlay(LevelMap obj)
		{
			// remove ghost tile if there is any
			OnCancelClick();
			// unselect
			if (IsSelected.Value) OnTileSelected(false);
			// assign a color and update render
			_controller.Tile.ShuffleColor();
			Renderer.material = Materials[_controller.Tile.TileColor];
			MovementLocked = false;
		}

		protected override void OnMouseDown()
		{
			if (_customLevelPlayer.IsPlaying)
			{
				base.OnMouseDown();
			}
			else
			{
				if (IsGhost) return;

				_onTileClicked?.OnNext(_controller.Tile);
			}
		}

		private void OnDeleteClick()
		{
			if (IsSelected.Value)
			{
				var neighbor = _controller.Tile.Neighbors.First();
				neighbor.Value.Neighbors.Remove(neighbor.Key.Opposite());
				neighbor.Value.HingePoints.Remove(neighbor.Key.Opposite());
				TileController.ChainRefreshPaths(neighbor.Value);
				Destroy(this.gameObject);
			}
		}

		private void OnCancelClick()
		{
			if (IsSelected.Value && _ghostTileView != null)
			{
				RemoveGhostNeighbor(_currentDirection);
				Destroy(_ghostTileView.gameObject);
				_ghostTileView = null;
				_levelEditorToolSet.UpdateButtons(_controller.Tile);
			}
		}

		private void OnConfirmClick()
		{
			if (IsSelected.Value && _ghostTileView != null)
			{
				_ghostTileView.IsGhost = false;
				_ghostTileView.Renderer.material = Renderer.material;
				_tileLevelMap.RegisterTile(_ghostTileView);
				_tileLevelMap.UpdateInstructions(this, _ghostTileView, _currentDirection, _currentRule);
				_ghostTileView.Start();
				_ghostTileView._currentDirection = _currentDirection;
				_ghostTileView._currentRule = _currentRule;

				_ghostTileView = null;
				_currentDirection = CardinalDirection.None;
				_levelEditorToolSet.UpdateButtons(_controller.Tile);
			}
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
			if (_customLevelPlayer.IsPlaying) return;

			IsSelected.Value = (tile == _controller.Tile) && !IsSelected.Value;

			if (IsSelected.Value)
			{
				_levelEditorToolSet.Delete.interactable = _controller.Tile.IsLeaf();
			}
		}

		private void InsertGhostNeighbor(CardinalDirection direction, NeighborWalkRule rule)
		{
			if (direction == CardinalDirection.None) return;

			if (_ghostTileView == null)
			{
				_ghostTileView = _tileFactory.NewInstance<LevelEditorTileView>();
				_ghostTileView.Controller.AdjustBounds(_ghostTileBounds);
				_ghostTileView.Renderer.material = _levelEditorToolSet.GhostMaterial;
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