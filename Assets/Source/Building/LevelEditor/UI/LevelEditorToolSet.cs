using System;
using System.Collections.Generic;
using System.Linq;
using TilesWalk.Building.Level;
using TilesWalk.General;
using TilesWalk.General.UI;
using TilesWalk.Tile.Rules;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TilesWalk.Building.LevelEditor.UI
{
	public class LevelEditorToolSet : MonoBehaviour
	{
		[Serializable]
		private class DirectionButton
		{
			[SerializeField] private CardinalDirection _direction;
			[SerializeField] private Button _button;

			public Button Button => _button;

			public CardinalDirection Direction => _direction;
		}

		[Serializable]
		private class NeighborWalkRuleButton
		{
			[SerializeField] private NeighborWalkRule _rule;
			[SerializeField] private Toggle _toggle;

			public Toggle Toggle => _toggle;

			public NeighborWalkRule Rule => _rule;
		}

		[Inject] private CustomLevelPlayer _customLevelPlayer;

		[Header("Insertion - Edit")] [SerializeField]
		private CanvasGroupBehaviour _insertionCanvas;
		[SerializeField] private Material _editorTileMaterial;
		[SerializeField] private Material _outlineMaterial;
		[SerializeField] private Material _ghostMaterial;

		[SerializeField] private List<DirectionButton> _directionInsertButtons;
		[SerializeField] private List<NeighborWalkRuleButton> _ruleInsertButtons;

		[SerializeField] private Button _confirm;
		[SerializeField] private Button _cancel;
		[SerializeField] private Button _delete;

		[Header("Save - Edit")] [SerializeField]
		private CanvasGroupBehaviour _saveLevelCanvas;

		public Material EditorTileMaterial => _editorTileMaterial;
		public Material OutlineMaterial => _outlineMaterial;
		public Material GhostMaterial => _ghostMaterial;

		public Button Confirm => _confirm;

		public Button Cancel => _cancel;

		public Button Delete => _delete;

		public CanvasGroupBehaviour SaveLevelCanvas => _saveLevelCanvas;

		public CanvasGroupBehaviour InsertionCanvas => _insertionCanvas;

		private void Start()
		{
			_customLevelPlayer.OnPlayAsObservable().Subscribe(OnCustomLevelPlay).AddTo(this);
			_customLevelPlayer.OnStopAsObservable().Subscribe(OnCustomLevelStop).AddTo(this);
		}

		private void OnCustomLevelStop(LevelMap obj)
		{
			InsertionCanvas.Show();
			SaveLevelCanvas.Show();
		}

		private void OnCustomLevelPlay(LevelMap level)
		{
			InsertionCanvas.Hide();
			SaveLevelCanvas.Hide();
		}

		public Button GetButton(CardinalDirection direction)
		{
			return _directionInsertButtons.First(x => x.Direction == direction).Button;
		}

		public Toggle GetToggle(NeighborWalkRule rule)
		{
			return _ruleInsertButtons.First(x => x.Rule == rule).Toggle;
		}

		public void UpdateButtons(Tile.Tile tile)
		{
			Cancel.interactable = true;
			Confirm.interactable = true;

			if (tile.Neighbors.Count > 0)
			{
				foreach (var button in _directionInsertButtons)
				{
					button.Button.interactable = !tile.Neighbors.Keys.Contains(button.Direction);
				}
			}
			// this tile is root so it can't be deleted
			else
			{
				foreach (var button in _directionInsertButtons)
				{
					button.Button.interactable = true;
				}
			}
		}
	}
}