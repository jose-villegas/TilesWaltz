using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TilesWalk.Building.Level;
using TilesWalk.Building.LevelEditor.UI;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Display;
using TilesWalk.Gameplay.Level;
using TilesWalk.General;
using TilesWalk.General.UI;
using TilesWalk.Tile;
using TilesWalk.Tile.Rules;
using UniRx;
using UnityEngine;
using Zenject;
using LevelTileView = TilesWalk.Tile.Level.LevelTileView;

namespace TilesWalk.Building.LevelEditor
{
	public class LevelEditorTileView : LevelTileView
	{
		[Inject] private LevelEditorToolSet _levelEditorToolSet;
		[Inject] private LevelTileViewFactory _levelTileFactory;
		[Inject] private CustomLevelPlayer _customLevelPlayer;
		[Inject] private CustomLevelsConfiguration _customLevelsConfiguration;
		[Inject] private Notice _notice;
		[Inject] private CanvasHoverListener _canvasHover;

		private Tuple<CardinalDirection, LevelEditorTileView> _ghostTileView;
		private Bounds _ghostTileBounds = new Bounds(Vector3.zero, new Vector3(1f, 0.3f, 1f));

		private NeighborWalkRule _currentRule = NeighborWalkRule.Plain;
		private CardinalDirection _currentDirection = CardinalDirection.None;
		private CanvasGroupBehaviour _guides;
		private Vector3? _sourcePosition = null;

		public BoolReactiveProperty IsSelected { get; } = new BoolReactiveProperty();
		public bool IsGhost { get; private set; } = false;

		public bool HasGhost => _ghostTileView != null && IsSelected.Value;

		public CardinalDirection GhostDirection => _ghostTileView.Item1;

		public NeighborWalkRule CurrentRule => _currentRule;

		public CardinalDirection CurrentDirection => _currentDirection;

		protected override void Start()
		{
			_guides = GetComponentInChildren<CanvasGroupBehaviour>(true);

			if (IsGhost) return;

			base.Start();

			_tileLevelMap.State = TileLevelMapState.EditorMode;
			Renderer.material = IsGhost ? _levelEditorToolSet.GhostMaterial : _levelEditorToolSet.EditorTileMaterial;

			_tileLevelMap.Trigger.OnTileClickedAsObservable().Subscribe(OnAnyTileClicked).AddTo(this);
			IsSelected.Subscribe(OnTileSelected).AddTo(this);

			_customLevelPlayer.OnPlayAsObservable().Subscribe(OnCustomLevelPlay).AddTo(this);
			_customLevelPlayer.OnStopAsObservable().Subscribe(OnCustomLevelStop).AddTo(this);

			_levelEditorToolSet.InsertionCanvas.Confirm.interactable = false;
			_levelEditorToolSet.InsertionCanvas.Cancel.interactable = false;
			_levelEditorToolSet.InsertionCanvas.Delete.interactable = false;

			_levelEditorToolSet.InsertionCanvas.Confirm.OnClickAsObservable().Subscribe(_ => OnConfirmClick())
				.AddTo(this);
			_levelEditorToolSet.InsertionCanvas.Cancel.OnClickAsObservable().Subscribe(_ => OnCancelClick())
				.AddTo(this);
			_levelEditorToolSet.InsertionCanvas.Delete.OnClickAsObservable().Subscribe(_ => OnDeleteClick())
				.AddTo(this);
			_levelEditorToolSet.ActionsCanvas.ShowGrid.onValueChanged.AsObservable().Subscribe(OnShowGridToggle)
				.AddTo(this);

			// subscribe to UI actions
			foreach (var value in Enum.GetValues(typeof(NeighborWalkRule)))
			{
				var enumValue = (NeighborWalkRule) value;
				var toggle = _levelEditorToolSet.InsertionCanvas.GetToggle(enumValue);
				toggle.OnValueChangedAsObservable().Subscribe(val =>
				{
					if (_customLevelPlayer.IsPlaying) return;

					_currentRule = val ? enumValue : _currentRule;

					if (val && IsSelected.Value)
					{
						// remove previous ghost
						RemoveGhostNeighbor(_currentDirection);
						InsertGhostNeighbor(_currentDirection, _currentRule);

						_levelEditorToolSet.InsertionCanvas.Confirm.interactable = true;
						_levelEditorToolSet.InsertionCanvas.Cancel.interactable = true;
					}
				}).AddTo(this);
			}

			foreach (var value in Enum.GetValues(typeof(CardinalDirection)))
			{
				var enumValue = (CardinalDirection) value;

				if (enumValue == CardinalDirection.None) continue;

				var button = _levelEditorToolSet.InsertionCanvas.GetButton(enumValue);
				button.OnClickAsObservable().Subscribe(_ =>
				{
					if (!IsSelected.Value) return;

					if (HasGhost && GhostDirection == enumValue)
					{
						var tileView = _ghostTileView.Item2;

						OnConfirmClick();

						IsSelected.Value = false;
						tileView.IsSelected.Value = true;
					}
					else
					{
						// remove previous ghost
						RemoveGhostNeighbor(_currentDirection);
						_currentDirection = enumValue;
						InsertGhostNeighbor(_currentDirection, _currentRule);
					}

					_levelEditorToolSet.InsertionCanvas.Confirm.interactable = true;
					_levelEditorToolSet.InsertionCanvas.Cancel.interactable = true;
				}).AddTo(this);
			}
		}

		private void OnShowGridToggle(bool val)
		{
			// first backup the position for the first time
			if (_sourcePosition == null)
			{
				_sourcePosition = this.transform.position;
			}

			if (val)
			{
				_levelEditorToolSet.SetEditorInterfaceState(LevelEditorToolSet.State.EditorActions);
			}
			else
			{
				_levelEditorToolSet.SetEditorInterfaceState(LevelEditorToolSet.State.EditorActionsAndInsertion);
			}

			_levelEditorToolSet.ActionsCanvas.ShowGrid.interactable = false;
			StartCoroutine(ToggleGridPosition(val)).GetAwaiter()
				.OnCompleted(() => _levelEditorToolSet.ActionsCanvas.ShowGrid.interactable = true);
		}

		private IEnumerator ToggleGridPosition(bool val)
		{
			var index = Controller.Tile.Index;

			if (_sourcePosition != null)
			{
				var source = _sourcePosition.Value;
				var target = new Vector3(index.x, index.y, index.z);

				if (!val)
				{
					target = source;
					source = Controller.Tile.Index;
				}

				var t = 0f;

				while (t < _animationSettings.GridAnimationTime)
				{
					transform.position = Vector3.Lerp(source, target, t / _animationSettings.GridAnimationTime);
					t += Time.deltaTime;
					yield return null;
				}
			}

			Transform childTrans = transform.Find("WireBox");

			if (childTrans != null) childTrans.gameObject.SetActive(val);

			yield return null;
		}

		private void OnCustomLevelStop(LevelMap obj)
		{
			_tileLevelMap.State = TileLevelMapState.EditorMode;
			// return blank material
			Renderer.material = IsGhost ? _levelEditorToolSet.GhostMaterial : _levelEditorToolSet.EditorTileMaterial;

			if (_levelEditorToolSet.ActionsCanvas.ShowGrid.isOn) 
			{
				_levelEditorToolSet.SetEditorInterfaceState(LevelEditorToolSet.State.EditorActions);

				Transform childTrans = transform.Find("WireBox");

				if (childTrans != null) childTrans.gameObject.SetActive(true);
			}
		}

		private void OnCustomLevelPlay(LevelMap obj)
		{
			// remove ghost tile if there is any
			OnCancelClick();
			// unselect
			if (IsSelected.Value) OnTileSelected(false);
			// assign a color and update render
			_controller.Tile.ShuffleColor();
			Renderer.material = _colorHandler.GetMaterial(_controller.Tile.TileColor);
			_tileLevelMap.State = TileLevelMapState.FreeMove;

			Transform childTrans = transform.Find("WireBox");

			if (childTrans != null) childTrans.gameObject.SetActive(false);
		}

		protected override void OnMouseDown()
		{
			if (_customLevelPlayer.IsPlaying)
			{
				base.OnMouseDown();
			}
			else
			{
				if (_canvasHover.IsUIOverride) return;

				if (IsGhost)
				{
					var view =
						_tileLevelMap.GetTileView(_controller.Tile.Neighbors.First().Value) as LevelEditorTileView;

					if (view != null)
					{
						view.OnConfirmClick();
					}

					return;
				}

				Trigger.OnTileClicked?.OnNext(_controller.Tile);
			}
		}

		private void OnDeleteClick()
		{
			if (IsSelected.Value)
			{
				var neighbors = _controller.Tile.Neighbors;

				foreach (var neighbor in neighbors)
				{
					neighbor.Value.Neighbors.Remove(neighbor.Key.Opposite());
					neighbor.Value.HingePoints.Remove(neighbor.Key.Opposite());
					TileController.ChainRefreshPaths(neighbor.Value);
				}

				_tileLevelMap.RemoveTile(this);
				Destroy(gameObject);
			}
		}

		private void OnCancelClick()
		{
			if (IsSelected.Value && _ghostTileView != null)
			{
				RemoveGhostNeighbor(_currentDirection);
				Destroy(_ghostTileView.Item2.transform.parent.gameObject);
				_ghostTileView = null;
				_levelEditorToolSet.InsertionCanvas.UpdateButtons(this);
			}
		}

		private void OnConfirmClick()
		{
			if (IsSelected.Value && _ghostTileView != null)
			{
				_ghostTileView.Item2.IsGhost = false;
				_ghostTileView.Item2.Renderer.material = Renderer.material;
				_tileLevelMap.RegisterTile(_ghostTileView.Item2);
				_tileLevelMap.UpdateInstructions(this, _ghostTileView.Item2, _currentDirection, _currentRule);

				// update indexes
				_tileLevelMap.UpdateIndexes(_ghostTileView.Item2);
				// update connection separators
				foreach (var tile in _tileLevelMap.HashToTile.Values)
				{
					if (tile != null) tile.CheckSeparator();
				}

				_ghostTileView.Item2.Start();
				_ghostTileView.Item2._currentDirection = _currentDirection;
				_ghostTileView.Item2._currentRule = _currentRule;

				_ghostTileView = null;
				_currentDirection = CardinalDirection.None;
				_levelEditorToolSet.InsertionCanvas.UpdateButtons(this);
			}
		}

		private void OnTileSelected(bool isSelected)
		{
			if (isSelected)
			{
				// update buttons interaction
				_levelEditorToolSet.InsertionCanvas.UpdateButtons(this);
				_guides.gameObject.SetActive(true);
				_guides.Show();

				// set outline for this tile
				var newMaterials = new[]
				{
					_levelEditorToolSet.EditorTileMaterial,
					_levelEditorToolSet.OutlineMaterial
				};

				Renderer.materials = newMaterials;
				// set canvas state
				_levelEditorToolSet.SetEditorInterfaceState(LevelEditorToolSet.State.EditorActionsAndInsertion);
			}
			else
			{
				_guides.Hide();
				if (Renderer.materials.Length > 1)
				{
					Renderer.materials = new[] {_levelEditorToolSet.EditorTileMaterial};
				}
			}
		}

		private void OnAnyTileClicked(Tile.Tile tile)
		{
			if (_customLevelPlayer.IsPlaying) return;

			if (!_levelEditorToolSet.ActionsCanvas.ShowUI.isOn) return;

			// user is unselecting the tile
			if (tile == _controller.Tile && IsSelected.Value)
			{
				// set canvas state
				_levelEditorToolSet.SetEditorInterfaceState(LevelEditorToolSet.State.EditorActions);
			}

			IsSelected.Value = (tile == _controller.Tile) && !IsSelected.Value;

			if (IsSelected.Value)
			{
				_levelEditorToolSet.InsertionCanvas.Delete.interactable = _controller.Tile.Neighbors.Count > 0;
			}
		}

		private void InsertGhostNeighbor(CardinalDirection direction, NeighborWalkRule rule)
		{
			if (direction == CardinalDirection.None) return;

			if (_ghostTileView == null)
			{
				_ghostTileView = new Tuple<CardinalDirection, LevelEditorTileView>
				(
					direction, _levelTileFactory.NewInstance<LevelEditorTileView>()
				);
				_ghostTileView.Item2.Controller.AdjustBounds(_ghostTileBounds);
				_ghostTileView.Item2.Renderer.material = _levelEditorToolSet.GhostMaterial;
			}
			else
			{
				_ghostTileView = new Tuple<CardinalDirection, LevelEditorTileView>
				(
					direction,
					_ghostTileView.Item2
				);
				_ghostTileView.Item2.transform.position = Vector3.zero;
				_ghostTileView.Item2.transform.rotation = Quaternion.identity;
				_ghostTileView.Item2.Controller = new TileController();
				_ghostTileView.Item2.Controller.AdjustBounds(_ghostTileBounds);
			}

			_ghostTileView.Item2.IsGhost = true;
			this.InsertNeighbor(direction, rule, _ghostTileView.Item2);
			_levelEditorToolSet.InsertionCanvas.UpdateButtons(this);

			// max amount of tiles reached
			if (_tileLevelMap.HashToTile.Count >= _customLevelsConfiguration.MaximumTilesPerLevel)
			{
				_notice.Configure("Maximum amount of tiles reached").Show(2f);
				OnCancelClick();
			}
			else if (_tileLevelMap.Indexes.ContainsKey(_ghostTileView.Item2.Controller.Tile.Index))
			{
				_notice.Configure("Two tiles cannot share the same index or position").Show(2f);
				OnCancelClick();
			}
			else if (_tileLevelMap.IsBreakingDistance(_ghostTileView.Item2))
			{
				_notice.Configure("Two tiles cannot share the same space").Show(2f);
				OnCancelClick();
			}
		}

		private void RemoveGhostNeighbor(CardinalDirection direction)
		{
			if (_ghostTileView != null)
			{
				_ghostTileView.Item2.transform.position = Vector3.zero;
				_ghostTileView.Item2.transform.rotation = Quaternion.identity;
				_ghostTileView.Item2.Controller = new TileController();
				_ghostTileView.Item2.Controller.AdjustBounds(_ghostTileBounds);
			}

			if (direction == CardinalDirection.None) return;

			_controller.RemoveNeighbor(direction);
			_levelEditorToolSet.InsertionCanvas.UpdateButtons(this);
		}
	}
}