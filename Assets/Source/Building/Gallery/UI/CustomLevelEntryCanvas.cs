using System;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Condition;
using TilesWalk.Gameplay.Persistence;
using TilesWalk.General.UI;
using TilesWalk.Map.Bridge;
using TilesWalk.Map.General;
using TilesWalk.Map.Scaffolding;
using TilesWalk.Tile.Level;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TilesWalk.Building.Gallery.UI
{
	/// <summary>
	/// This class handles the panel view for all the custom entries
	/// in the game level gallery
	/// </summary>
	public class CustomLevelEntryCanvas : MonoBehaviour
	{
		[Inject] private LevelBridge _bridge;
		[Inject] private ShareLevelCanvas _shareCanvas;
		[Inject] private TileViewLevelMap _tileLevelMap;
		[Inject] private LevelMapPreviewRenderCamera _previewCamera;
		[Inject] private MapProviderSolver _solver;
		[Inject] private Confirmation _confirmation;
		[Inject] private GameSave _gameSave;

		[SerializeField] private LevelNameRequestHandler _levelRequest;
		[SerializeField] private CanvasGroupBehaviour _timeCanvas;
		[SerializeField] private CanvasGroupBehaviour _movesCanvas;
        [SerializeField] private GameObject _loadingContainer;
		[SerializeField] private GameObject _mapPreviewContainer;
		[SerializeField] private RawImage _mapPreview;
		[SerializeField] private Button _delete;
		[SerializeField] private Button _edit;
		[SerializeField] private Button _addToUser;
		[SerializeField] private Button _play;
		[SerializeField] private Button _share;

		public LevelNameRequestHandler LevelRequest => _levelRequest;

        public static int counter = 0;

		private void Start()
		{
			if (_solver.Source == Provider.UserMaps)
			{
				_edit.gameObject.SetActive(true);
				_addToUser.gameObject.SetActive(false);
			}
			else if (_solver.Source == Provider.ImportedMaps)
			{
				_edit.gameObject.SetActive(false);
				_addToUser.gameObject.SetActive(true);
			}

			_levelRequest.Name.Subscribe(UpdateCanvas).AddTo(this);
			_edit.onClick.AsObservable().Subscribe(OnEditClick).AddTo(this);
			_play.onClick.AsObservable().Subscribe(OnPlayClick).AddTo(this);
			_delete.onClick.AsObservable().Subscribe(OnDeleteClick).AddTo(this);
			_share.onClick.AsObservable().Subscribe(OnShareClick).AddTo(this);
			_addToUser.onClick.AsObservable().Subscribe(OnAddToUserMapsClicked).AddTo(this);

            _loadingContainer.gameObject.SetActive(true);
        }

        public void RefreshMapPreview()
        {
            _tileLevelMap.Reset();
            _tileLevelMap.BuildTileMap<LevelTileView>(_levelRequest.Map);
            _mapPreview.texture = _previewCamera.GetCurrentRender();
            _tileLevelMap.Reset();

            _loadingContainer.gameObject.SetActive(false);
            _mapPreviewContainer.gameObject.SetActive(true);
		}

		private void OnAddToUserMapsClicked(Unit u)
		{
			var alreadyExisting = _gameSave.UserMaps.Exist(_levelRequest.Map.Id);

			if (!alreadyExisting)
			{
				_confirmation.Configure("Add this map to user maps?",
					() => { _gameSave.UserMaps.Insert(_levelRequest.Map, _levelRequest.Condition); }).Show();
			}
			else
			{
				_confirmation.Configure(
					$"This will replace your map named {_levelRequest.Map.Id}, continue?",
					() => { _gameSave.UserMaps.Insert(_levelRequest.Map, _levelRequest.Condition); }).Show();
			}
		}

		private void OnEditClick(Unit u)
		{
			_bridge.Payload = new LevelBridgePayload(_levelRequest.Map, _levelRequest.Condition);
		}

		private void OnPlayClick(Unit u)
		{
			_bridge.Payload = new LevelBridgePayload(_levelRequest.Map, _levelRequest.Condition);
		}

		private void OnDeleteClick(Unit u)
		{
			_confirmation.Configure(() =>
			{
				_solver.Provider.Collection.Remove(_levelRequest.Name.Value);
			}).Show();
		}

		private void OnShareClick(Unit u)
		{
			_shareCanvas.Configure(_mapPreview.texture, _levelRequest.Map).Show();
		}

		private void UpdateCanvas(string val)
		{
			_timeCanvas.Show();
			_movesCanvas.Show();

			switch (_levelRequest.Map.FinishCondition)
			{
				case FinishCondition.TimeLimit:
					_movesCanvas.Hide();
					break;
				case FinishCondition.MovesLimit:
					_timeCanvas.Hide();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}