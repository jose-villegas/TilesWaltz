using System;
using TilesWalk.Gameplay.Condition;
using TilesWalk.General.UI;
using TilesWalk.Map.Scaffolding;
using UniRx;
using UnityEngine;

namespace TilesWalk.Building.LevelEditor.UI.Gallery
{
    public class CustomLevelEntryCanvas : MonoBehaviour
    {
	    [SerializeField] private LevelNameRequestHandler _levelRequest;
	    [SerializeField] private CanvasGroupBehaviour _timeCanvas;
	    [SerializeField] private CanvasGroupBehaviour _movesCanvas;

	    public LevelNameRequestHandler LevelRequest => _levelRequest;

	    private void Start()
	    {
		    _levelRequest.Name.Subscribe(UpdateCanvas).AddTo(this);
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