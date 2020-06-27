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
		[Inject] private CustomLevelPlayer _customLevelPlayer;

		[Header("Materials")] [SerializeField] private Material _editorTileMaterial;
		[SerializeField] private Material _outlineMaterial;
		[SerializeField] private Material _ghostMaterial;

		[Header("Insertion - Edit")] [SerializeField]
		private TileInsertionModeCanvas _insertionCanvas;

		[Header("Save - Edit")] [SerializeField]
		private LevelEditorActionsCanvas _editorActionsCanvas;

		public Material EditorTileMaterial => _editorTileMaterial;
		public Material OutlineMaterial => _outlineMaterial;
		public Material GhostMaterial => _ghostMaterial;

		public LevelEditorActionsCanvas ActionsCanvas => _editorActionsCanvas;

		public TileInsertionModeCanvas InsertionCanvas => _insertionCanvas;

		public enum State
		{
			NoInterface,
			EditorActions,
			EditorInsertionTools,
			EditorActionsAndInsertion,
		}

		private void Start()
		{
			_customLevelPlayer.OnPlayAsObservable().Subscribe(OnCustomLevelPlay).AddTo(this);
			_customLevelPlayer.OnStopAsObservable().Subscribe(OnCustomLevelStop).AddTo(this);
		}

		private void OnCustomLevelStop(LevelMap obj)
		{
			SetEditorInterfaceState(State.EditorActionsAndInsertion);
		}

		private void OnCustomLevelPlay(LevelMap level)
		{
			SetEditorInterfaceState(State.NoInterface);
		}

		public void SetEditorInterfaceState(State state)
		{
			switch (state)
			{
				case State.NoInterface:
					InsertionCanvas.Hide();
					ActionsCanvas.Hide();
					break;
				case State.EditorActions:
					if (!ActionsCanvas.IsVisible) ActionsCanvas.Show();
					InsertionCanvas.Hide();
					break;
				case State.EditorInsertionTools:
					ActionsCanvas.Hide();
					if (!InsertionCanvas.IsVisible) InsertionCanvas.Show();
					break;
				case State.EditorActionsAndInsertion:
					if (!InsertionCanvas.IsVisible) InsertionCanvas.Show();
					if (!ActionsCanvas.IsVisible) ActionsCanvas.Show();
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(state), state, null);
			}
		}
	}
}