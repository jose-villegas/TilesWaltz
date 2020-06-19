using TilesWalk.General.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TilesWalk.Building.LevelEditor.UI
{
	public class LevelEditorActionsCanvas : CanvasGroupBehaviour
	{
		[Inject] private LevelEditorToolSet _levelEditorToolSet;

		[SerializeField] private Button _edit;
		[SerializeField] private Button _save;

		public Button Edit => _edit;

		public Button Save => _save;

		public void Start()
		{
		}
	}
}