using System;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Condition;
using TilesWalk.Gameplay.Display;
using TilesWalk.General.UI;
using TilesWalk.Map.Bridge;
using TilesWalk.Map.General;
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
		[Inject] private TileViewLevelMap _tileLevelMap;
		[Inject] private MapProviderSolver _solver;
		[Inject] private Notice _notice;
		[Inject] private Confirmation _confirmation;

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

		private Subject<LevelMap> _onLevelMapSaved;
		private string _originalName = string.Empty;
		private bool _maxNoticeShown;

		private void Awake()
		{
			Hide();
			_solver.InstanceProvider(gameObject);

			if (_solver.Provider.Collection.AvailableMaps != null &&
			    _solver.Provider.Collection.AvailableMaps.Count >= _solver.Provider.MaximumLevels &&
			    _solver.Provider.Collection.AvailableMaps.FindIndex(x => x.Id == _bridge.Payload.Level.Id) < 0)
			{
				_save.interactable = false;
			}

			_save.onClick.AsObservable().Subscribe(OnSaveConfirm).AddTo(this);
			_cancel.onClick.AsObservable().Subscribe(OnCancelSave).AddTo(this);
			_exit.onClick.AsObservable().Subscribe(OnCancelSave).AddTo(this);
		}

		private void OnCancelSave(Unit u)
		{
			Hide();
		}

		public void OnSaveConfirm(Unit u)
		{
			if (!_save.interactable) return;

			// check if the name changed
			if (_tileLevelMap.Map.Id != _originalName)
			{
				// check if another map with this name exists
				if (_solver.Provider.Collection.Exist(_tileLevelMap.Map.Id))
				{
					_confirmation.Configure("A map with the same name already exists, replace?", () =>
					{
						_originalName = _tileLevelMap.Map.Id;
						OnSaveConfirm(u);
					}).Show();

					return;
				}
			}

			var map = new LevelMap(_tileLevelMap.Map);

			if (_movesToggle.isOn)
			{
				_solver.Provider.Collection.Insert(map, new MovesFinishCondition
				(
					_tileLevelMap.Map.Id,
					int.Parse(_movesField.text)
				));
			}
			else
			{
				_solver.Provider.Collection.Insert(map, new TimeFinishCondition
				(
					_tileLevelMap.Map.Id,
					float.Parse(_secondsField.text)
				));
			}

			_notice
				.Configure("Saved Successfully", NoticePriority.Message, GameColor.Action, GameColor.Strong)
				.Show(1f);
			_onLevelMapSaved?.OnNext(map);
			Hide();
		}

		private void Start()
		{
			_titleField.onValueChanged.AsObservable()
				.Subscribe(val => _tileLevelMap.Map.Id = val).AddTo(this);
			_targetPointsField.onValueChanged.AsObservable()
				.Subscribe(val => _tileLevelMap.Map.Target = int.Parse(val)).AddTo(this);
			_movesToggle.onValueChanged.AsObservable()
				.Subscribe(val =>
				{
					if (val)
					{
						_tileLevelMap.Map.FinishCondition = FinishCondition.MovesLimit;
					}
				}).AddTo(this);
			_timeToggle.onValueChanged.AsObservable()
				.Subscribe(val =>
				{
					if (val)
					{
						_tileLevelMap.Map.FinishCondition = FinishCondition.TimeLimit;
					}
				}).AddTo(this);

			if (_tileLevelMap.LoadOption == LevelLoadOptions.FromBridgeEditorMode && _bridge.Payload != null)
			{
				_originalName = _bridge.Payload.Level.Id;
				FillCanvas();
			}
		}

		private void FillCanvas()
		{
			_titleField.text = _bridge.Payload.Level.Id;

			// disable input for title if reached maximum amount of levels
			if (_solver.Provider.Collection.AvailableMaps.Count >= _solver.Provider.MaximumLevels)
			{
				_titleField.interactable = false;
			}

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

		public override void Show()
		{
			base.Show();

			if (!_maxNoticeShown && _solver.Provider.Collection.AvailableMaps != null &&
			    _solver.Provider.Collection.AvailableMaps.Count >= _solver.Provider.MaximumLevels)
			{
				_maxNoticeShown = true;
				_titleField.interactable = false;
				_notice.Configure("Maximum amount of custom levels reached, disabled title edit").Show(1.5f);
			}
		}

		private void OnSaveRequestClick(Unit u)
		{
			Show();
		}

		public IObservable<LevelMap> OnLevelMapSavedAsObservable()
		{
			return _onLevelMapSaved = _onLevelMapSaved ?? new Subject<LevelMap>();
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			base.RaiseOnCompletedOnDestroy();
			_onLevelMapSaved?.OnCompleted();
		}
	}
}