using System;
using System.Collections.Generic;
using System.Linq;
using TilesWalk.General;
using TilesWalk.General.UI;
using TilesWalk.Tile.Rules;
using UnityEngine;
using UnityEngine.UI;

namespace TilesWalk.Building.LevelEditor.UI
{
	public class TileInsertionModeCanvas : CanvasGroupBehaviour
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

		private CanvasGroupBehaviour _insertionCanvas;

		[Header("Tile Direction")] [SerializeField]
		private List<DirectionButton> _directionInsertButtons;

		[Header("Tile Orientation")] [SerializeField]
		private List<NeighborWalkRuleButton> _ruleInsertButtons;

		[Header("Actions")] [SerializeField] private Button _confirm;
		[SerializeField] private Button _cancel;
		[SerializeField] private Button _delete;

		public Button Confirm => _confirm;

		public Button Cancel => _cancel;

		public Button Delete => _delete;

		public Button GetButton(CardinalDirection direction)
		{
			return _directionInsertButtons.First(x => x.Direction == direction).Button;
		}

		public Toggle GetToggle(NeighborWalkRule rule)
		{
			return _ruleInsertButtons.First(x => x.Rule == rule).Toggle;
		}

		public void UpdateButtons(LevelEditorTileView tile)
		{
			Cancel.interactable = true;
			Confirm.interactable = true;

			if (tile.Controller.Tile.Neighbors.Count > 0)
			{
				foreach (var button in _directionInsertButtons)
				{
					button.Button.interactable = !tile.Controller.Tile.Neighbors.Keys.Contains(button.Direction);
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

			_confirm.interactable = _cancel.interactable = tile.HasGhost;
		}
	}
}