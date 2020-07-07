using System;
using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.General.UI;
using TilesWalk.Tile;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using LevelTileView = TilesWalk.Tile.Level.LevelTileView;

namespace TilesWalk.Building.LevelEditor.UI
{
	public class LevelEditorActionsCanvas : CanvasGroupBehaviour
	{
		[Inject] private LevelEditorToolSet _levelEditorToolSet;
		[Inject] private SaveLevelCanvas _saveLevelCanvas;
		[Inject] private TileViewLevelMap _levelMap;

		[SerializeField] private Button _edit;
		[SerializeField] private Button _save;
		[SerializeField] private Image _notification;
		[SerializeField] private TextMeshProUGUI _counter;

		private int _changesCounter = 0;

		public Button Edit => _edit;

		public Button Save => _save;

		public void Start()
		{
			_save.gameObject.SetActive(false);
			_notification.gameObject.SetActive(false);

			_edit.onClick.AsObservable().Subscribe(OnEditClick).AddTo(this);
			_save.onClick.AsObservable().Subscribe(OnQuickSaveClick).AddTo(this);

			_levelMap.OnTileRegisteredAsObservable().Subscribe(OnTileRegistered).AddTo(this);

			_saveLevelCanvas.OnLevelMapSavedAsObservable().Subscribe(OnLevelMapSaved).AddTo(this);
		}

		private void OnTileRegistered(LevelTileView tile)
		{
			_notification.gameObject.SetActive(true);
			_changesCounter++;

			if (_changesCounter < 10)
			{
				_counter.text = _changesCounter.Localize();
			}
			else
			{
				_counter.text = "+9";
			}
		}

		private void OnEditClick(Unit u)
		{
			_saveLevelCanvas.Show();
		}

		private void OnQuickSaveClick(Unit u)
		{
			_saveLevelCanvas.OnSaveConfirm(u);
		}

		private void OnLevelMapSaved(LevelMap map)
		{
			_save.gameObject.SetActive(true);
			_notification.gameObject.SetActive(false);
			_counter.text = "0";
			_changesCounter = 0;
		}
	}
}