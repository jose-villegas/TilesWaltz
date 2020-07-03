using System;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Condition;
using TilesWalk.General.UI;
using TilesWalk.Map.General;
using TilesWalk.Map.Scaffolding;
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

		[SerializeField] private Color _background;
		[SerializeField] private Color _values;
		[SerializeField] private LevelNameRequestHandler _levelRequest;
		[SerializeField] private RawImage _mapPreview;
		[SerializeField] private RawImage _qrCode;
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

		private static Color32[] Encode(string textForEncoding, int width, int height)
		{
			var writer = new BarcodeWriter
			{
				Format = BarcodeFormat.QR_CODE,
				Options = new QrCodeEncodingOptions
				{
					Height = height,
					Width = width,
				}
			};

			return writer.Write(textForEncoding);
		}

		private Texture2D GenerateQR(string text)
		{
			var encoded = new Texture2D(256, 256);
			var color32 = Encode(text, encoded.width, encoded.height);
			encoded.SetPixels32(color32);
			encoded.Apply();
			return encoded;
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
			_qrCode.texture = GenerateQR(parsedToQR);

			LevelMap.FromQRString(parsedToQR, out var testLevelMap, out var testCondition);
			Debug.Log("Here");
		}
	}
}