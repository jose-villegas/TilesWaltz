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

namespace TilesWalk.Building.Gallery.UI
{
	public class CustomLevelEntryCanvas : MonoBehaviour
	{
		[Inject] private LevelBridge _bridge;

		// Used for rendering the map preview
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
		[SerializeField] private Button _play;

		public LevelNameRequestHandler LevelRequest => _levelRequest;

		private void Start()
		{
			_levelRequest.Name.Subscribe(UpdateCanvas).AddTo(this);
			_edit.onClick.AsObservable().Subscribe(OnEditClick).AddTo(this);
			_play.onClick.AsObservable().Subscribe(OnPlayClick).AddTo(this);
			_delete.onClick.AsObservable().Subscribe(OnDeleteClick).AddTo(this);

			_levelMap.BuildTileMap<TileView>(_levelRequest.Map);
			_mapPreview.texture = _previewCamera.GetCurrentRender();
			_levelMap.Reset();
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