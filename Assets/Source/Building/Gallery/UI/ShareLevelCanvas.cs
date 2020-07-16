using System;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Condition;
using TilesWalk.General.QR;
using TilesWalk.General.UI;
using TilesWalk.Map.General;
using TilesWalk.Map.Scaffolding;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using ZXing;
using ZXing.QrCode;

namespace TilesWalk.Building.Gallery.UI
{
	public class ShareLevelCanvas : CanvasGroupBehaviour
	{
		[Inject] private MapProviderSolver _solver;

		[SerializeField] private LevelNameRequestHandler _levelRequest;
		[SerializeField] private RawImage _mapPreview;
		[SerializeField] private RawImage _qrCode;
		[SerializeField] private TMP_InputField _code;
		[SerializeField] private CanvasGroupBehaviour _timeCanvas;
		[SerializeField] private CanvasGroupBehaviour _movesCanvas;

		private void Start()
		{
			_levelRequest.Name.Subscribe(UpdateCanvas).AddTo(this);
		}

		public ShareLevelCanvas Configure(Texture mapTexture, LevelMap map)
		{
			_levelRequest.Name.Value = map.Id;
			_mapPreview.texture = mapTexture;

			return this;
		}

		private void UpdateCanvas(string val)
		{
			if (_levelRequest.Map == null) return;

			_timeCanvas.Show();
			_movesCanvas.Show();
			var limit = 0;

			switch (_levelRequest.Map.FinishCondition)
			{
				case FinishCondition.TimeLimit:
					var timeCondition = _solver.Provider.Collection.TimeFinishConditions
						.Find(x => x.Id == _levelRequest.Map.Id);

					if (timeCondition != null) limit = Mathf.CeilToInt(timeCondition.Limit);

					_movesCanvas.Hide();
					break;
				case FinishCondition.MovesLimit:
					var movesCondition = _solver.Provider.Collection.MovesFinishConditions
						.Find(x => x.Id == _levelRequest.Map.Id);

					if (movesCondition != null) limit = Mathf.CeilToInt(movesCondition.Limit);

					_timeCanvas.Hide();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			var parsedToQR = _levelRequest.Map.ToQRString(limit);
			_qrCode.texture = TextQRConverter.GenerateTexture(parsedToQR);
			_code.text = parsedToQR;

            _code.onValueChanged.AsObservable().Subscribe(_ =>
            {
                _code.text = parsedToQR;
            }).AddTo(this);
        }
	}
}