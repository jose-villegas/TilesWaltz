using System;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Condition;
using TilesWalk.General.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TilesWalk.Building.LevelEditor.UI
{
	public class SaveLevelCanvas : CanvasGroupBehaviour
	{
		[Inject] private LevelEditorToolSet _levelEditorToolSet;
		[Inject] private TileViewLevelMap _tileViewLevelMap;

		[SerializeField] private TMP_InputField _titleField;

		[Header("Completion")] [SerializeField]
		private TMP_InputField _targetPointsField;

		[Header("Limits")] [SerializeField] private Toggle _movesToggle;
		[SerializeField] private Toggle _timeToggle;
		[SerializeField] private TMP_InputField _movesField;
		[SerializeField] private TMP_InputField _secondsField;
		[Header("Actions")] [SerializeField] private Button _save;
		[SerializeField] private Button _cancel;

		private void Awake()
		{
			Hide();

			_levelEditorToolSet.ActionsCanvas.Save.onClick.AsObservable().Subscribe(OnSaveRequestClick).AddTo(this);
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
		}

		private void OnSaveRequestClick(Unit u)
		{
			Show();
		}
	}
}