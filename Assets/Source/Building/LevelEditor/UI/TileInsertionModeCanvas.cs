using System.Collections.Generic;
using System.Linq;
using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.General;
using TilesWalk.General.UI;
using TilesWalk.Tile.Rules;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TilesWalk.Building.LevelEditor.UI
{
	public class TileInsertionModeCanvas : CanvasGroupBehaviour
	{
		[Inject] private TileViewLevelMap _levelMap;

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
					button.Button.image.enabled = button.Button.interactable =
						tile.Controller.Tile.IsValidInsertion(button.Direction, tile.CurrentRule) ||
						tile.HasGhost && button.Direction == tile.GhostDirection;
				}
			}
			// this tile is root so it can't be deleted
			else
			{
				foreach (var button in _directionInsertButtons)
				{
					button.Button.image.enabled = true;
					button.Button.interactable = true;
				}
			}

			_confirm.interactable = _cancel.interactable = tile.HasGhost;
		}
	}
}