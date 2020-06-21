using System;
using TilesWalk.Gameplay.Condition;
using TilesWalk.General.UI;
using TilesWalk.Map.Bridge;
using TilesWalk.Map.Scaffolding;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TilesWalk.Building.LevelEditor.UI.Gallery
{
    public class CustomLevelEntryCanvas : MonoBehaviour
    {
	    [Inject] private LevelBridge _bridge;

	    [SerializeField] private LevelNameRequestHandler _levelRequest;
	    [SerializeField] private CanvasGroupBehaviour _timeCanvas;
	    [SerializeField] private CanvasGroupBehaviour _movesCanvas;
	    [SerializeField] private Button _edit;
	    [SerializeField] private Button _play;

	    public LevelNameRequestHandler LevelRequest => _levelRequest;

	    private void Start()
	    {
		    _levelRequest.Name.Subscribe(UpdateCanvas).AddTo(this);
		    _edit.onClick.AsObservable().Subscribe(OnEditClick).AddTo(this);
		    _play.onClick.AsObservable().Subscribe(OnPlayClick).AddTo(this);
		}

		private void OnEditClick(Unit u)
		{
			_bridge.Payload = new LevelBridgePayload(_levelRequest.Map, _levelRequest.Condition);
		}

		private void OnPlayClick(Unit u)
		{
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