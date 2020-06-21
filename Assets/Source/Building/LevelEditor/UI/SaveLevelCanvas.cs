using System;
using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Condition;
using TilesWalk.Gameplay.Persistence;
using TilesWalk.General.UI;
using TilesWalk.Map.Bridge;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TilesWalk.Building.LevelEditor.UI
{
	public class SaveLevelCanvas : CanvasGroupBehaviour
	{
		[Inject] private LevelBridge _bridge;
		[Inject] private LevelEditorToolSet _levelEditorToolSet;
		[Inject] private TileViewLevelMap _tileViewLevelMap;
		[Inject] private GameSave _gameSave;

		[SerializeField] private TMP_InputField _titleField;

		[Header("Completion")] [SerializeField]
		private TMP_InputField _targetPointsField;

		[Header("Limits")] [SerializeField] private Toggle _movesToggle;
		[SerializeField] private Toggle _timeToggle;
		[SerializeField] private TMP_InputField _movesField;
		[SerializeField] private TMP_InputField _secondsField;
		[Header("Actions")] [SerializeField] private Button _save;
		[SerializeField] private Button _cancel;
		[SerializeField] private Button _exit;

		private void Awake()
		{
			Hide();

			_levelEditorToolSet.ActionsCanvas.Edit.onClick.AsObservable().Subscribe(OnSaveRequestClick).AddTo(this);
			_levelEditorToolSet.ActionsCanvas.Save.onClick.AsObservable().Subscribe(OnSaveConfirm).AddTo(this);
			_save.onClick.AsObservable().Subscribe(OnSaveConfirm).AddTo(this);
			_cancel.onClick.AsObservable().Subscribe(OnCancelSave).AddTo(this);
			_exit.onClick.AsObservable().Subscribe(OnCancelSave).AddTo(this);
		}

		private void OnCancelSave(Unit u)
		{
			Hide();
		}

		private void OnSaveConfirm(Unit u)
		{
			var map = new LevelMap(_tileViewLevelMap.LevelMap);

			if (_movesToggle.isOn)
			{
				_gameSave.UserMaps.Insert(map, new MovesFinishCondition
				(
					_tileViewLevelMap.LevelMap.Id,
					int.Parse(_movesField.text)
				));
			}
			else
			{
				_gameSave.UserMaps.Insert(map, new TimeFinishCondition
				(
					_tileViewLevelMap.LevelMap.Id,
					float.Parse(_secondsField.text)
				));
			}

			Hide();
		}

		private void Start()
		{
			_titleField.onValueChanged.AsObservable()
				.Subscribe(val => _tileViewLevelMap.LevelMap.Id = val).AddTo(this);
			_targetPointsField.onValueChanged.AsObservable()
				.Subscribe(val => _tileViewLevelMap.LevelMap.Target = int.Parse(val)).AddTo(this);
			_movesToggle.onValueChanged.AsObservable()
				.Subscribe(val =>
				{
					if (val)
					{
						_tileViewLevelMap.LevelMap.FinishCondition = FinishCondition.MovesLimit;
					}
				}).AddTo(this);
			_timeToggle.onValueChanged.AsObservable()
				.Subscribe(val =>
				{
					if (val)
					{
						_tileViewLevelMap.LevelMap.FinishCondition = FinishCondition.TimeLimit;
					}
				}).AddTo(this);

			if (_tileViewLevelMap.LoadOption == LevelLoadOptions.FromBridgeEditorMode)
			{
				FillCanvas();
			}
		}

		private void FillCanvas()
		{
			_titleField.text = _bridge.Payload.Level.Id;
			_targetPointsField.text = _bridge.Payload.Level.Target.ToString();

			_movesToggle.isOn = _bridge.Payload.Level.FinishCondition == FinishCondition.MovesLimit;
			_timeToggle.isOn = _bridge.Payload.Level.FinishCondition == FinishCondition.TimeLimit;

			switch (_bridge.Payload.Level.FinishCondition)
			{
				case FinishCondition.TimeLimit:
					if (_bridge.Payload.Condition is TimeFinishCondition t)
					{
						_secondsField.text = t.Limit.ToString();
					}
					break;
				case FinishCondition.MovesLimit:
					if (_bridge.Payload.Condition is MovesFinishCondition m)
					{
						_movesField.text = m.Limit.ToString();
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void OnSaveRequestClick(Unit u)
		{
			Show();
		}
	}
}