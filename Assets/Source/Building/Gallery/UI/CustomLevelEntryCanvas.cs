using System;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Condition;
using TilesWalk.General.UI;
using TilesWalk.Map.Bridge;
using TilesWalk.Map.General;
using TilesWalk.Map.Scaffolding;
using TilesWalk.Tile;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using LevelTileView = TilesWalk.Tile.Level.LevelTileView;

namespace TilesWalk.Building.Gallery.UI
{
	public class CustomLevelEntryCanvas : MonoBehaviour
	{
		[Inject] private LevelBridge _bridge;
		[Inject] private ShareLevelCanvas _shareCanvas;
		[Inject] private TileViewLevelMap _levelMap;
		[Inject] private LevelMapPreviewRenderCamera _previewCamera;
		[Inject] private MapProviderSolver _solver;
		[Inject] private Confirmation _confirmation;

		[SerializeField] private LevelNameRequestHandler _levelRequest;
		[SerializeField] private CanvasGroupBehaviour _timeCanvas;
		[SerializeField] private CanvasGroupBehaviour _movesCanvas;
		[SerializeField] private RawImage _mapPreview;
		[SerializeField] private Button _delete;
		[SerializeField] private Button _edit;
		[SerializeField] private Button _addToUser;
		[SerializeField] private Button _play;
		[SerializeField] private Button _share;
		[SerializeField] private UserLevelMapsProvider _userMaps;

		public LevelNameRequestHandler LevelRequest => _levelRequest;

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

			if (_userMaps.Collection.AvailableMaps.Count >= _userMaps.MaximumLevels)
			{
				_addToUser.interactable = false;
			}

			_levelRequest.Name.Subscribe(UpdateCanvas).AddTo(this);
			_edit.onClick.AsObservable().Subscribe(OnEditClick).AddTo(this);
			_play.onClick.AsObservable().Subscribe(OnPlayClick).AddTo(this);
			_delete.onClick.AsObservable().Subscribe(OnDeleteClick).AddTo(this);
			_share.onClick.AsObservable().Subscribe(OnShareClick).AddTo(this);
			_addToUser.onClick.AsObservable().Subscribe(OnAddToUserMapsClicked).AddTo(this);

			_levelMap.BuildTileMap<LevelTileView>(_levelRequest.Map);
			_mapPreview.texture = _previewCamera.GetCurrentRender();
			_levelMap.Reset();
		}

		private void OnAddToUserMapsClicked(Unit u)
		{
			var alreadyExisting = _userMaps.Collection.Exist(_levelRequest.Map.Id);

			if (!alreadyExisting)
			{
				_confirmation.Configure("Add this map to user maps?",
					() => { _userMaps.Collection.Insert(_levelRequest.Map, _levelRequest.Condition); }).Show();
			}
			else
			{
				_confirmation.Configure(
					$"This will replace your map named {_levelRequest.Map.Id}, continue?",
					() => { _userMaps.Collection.Insert(_levelRequest.Map, _levelRequest.Condition); }).Show();
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
				Destroy(gameObject);
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