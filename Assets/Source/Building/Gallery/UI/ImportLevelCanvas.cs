using System;
using System.Collections;
using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Condition;
using TilesWalk.General.QR;
using TilesWalk.General.UI;
using TilesWalk.Map.General;
using TilesWalk.Tile;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using ZXing;

namespace TilesWalk.Building.Gallery.UI
{
	public class ImportLevelCanvas : CanvasGroupBehaviour
	{
		[Inject] private TileViewLevelMap _levelMap;
		[Inject] private LevelMapPreviewRenderCamera _previewCamera;
		[Inject] private MapProviderSolver _solver;

		[SerializeField] private Animator _animator;
		[SerializeField] private Image _cameraRenderer;

		[Header("Content")] [SerializeField] private TextMeshProUGUI _title;
		[SerializeField] private TextMeshProUGUI _points;

		[SerializeField] private RawImage _mapPreview;
		[SerializeField] private RawImage _qr;

		[SerializeField] private CanvasGroupBehaviour _timeCanvas;
		[SerializeField] private TextMeshProUGUI _time;
		[SerializeField] private CanvasGroupBehaviour _movesCanvas;
		[SerializeField] private TextMeshProUGUI _moves;

		private WebCamTexture _cameraTexture;

		private LevelMap _map;
		private MapFinishCondition _condition;
		private bool _isMapRead;


		private void Start()
		{
			StartCoroutine(RequestCamera()).GetAwaiter().OnCompleted(() =>
			{
				Observable.Interval(TimeSpan.FromMilliseconds(100)).Subscribe(_ => ReadQR()).AddTo(this);
			});
		}

		private IEnumerator RequestCamera()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);

			if (Application.HasUserAuthorization(UserAuthorization.WebCam))
			{
				_cameraTexture = new WebCamTexture();
				_cameraTexture.requestedHeight = 512;
				_cameraTexture.requestedWidth = 512;

				if (_cameraTexture != null)
				{
					_cameraTexture.Play();
					_cameraRenderer.material = new Material(_cameraRenderer.material) {mainTexture = _cameraTexture};
				}
			}
#elif UNITY_EDITOR
			_cameraTexture = new WebCamTexture {requestedHeight = 512, requestedWidth = 512};

			if (_cameraTexture != null)
			{
				_cameraTexture.Play();
				_cameraRenderer.material = new Material(_cameraRenderer.material) {mainTexture = _cameraTexture};
				yield break;
			}
#endif
		}

		public override void Show()
		{
			base.Show();
			StartCoroutine(RequestCamera());
		}

		private void ReadQR()
		{
			if (_isMapRead) return;

			try
			{
				if (_cameraTexture == null || !_cameraTexture.isPlaying)
				{
					StartCoroutine(RequestCamera());
					return;
				}

				IBarcodeReader barcodeReader = new BarcodeReader();

				// decode the current frame
				var result = barcodeReader.Decode(_cameraTexture.GetPixels32(), _cameraTexture.width,
					_cameraTexture.height);

				if (result != null)
				{
					LevelMap.FromQRString(result.Text, out _map, out _condition);
					_animator.SetTrigger("ScanningDone");
					_isMapRead = true;
					_cameraTexture.Stop();
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning(ex.Message);
			}
		}

		private void UpdateCanvas()
		{
			_title.text = _map.Id;
			_points.text = _map.Target.Localize();

			_movesCanvas.Show();
			_timeCanvas.Show();

			int limit = 0;

			switch (_map.FinishCondition)
			{
				case FinishCondition.TimeLimit:
					_movesCanvas.Hide();

					if (_condition is TimeFinishCondition timeCond)
					{
						var time = TimeSpan.FromSeconds(timeCond.Limit);
						limit = Mathf.CeilToInt(timeCond.Limit);
						_time.text = string.Format("{0:mm\\:ss}", time);
					}

					break;
				case FinishCondition.MovesLimit:
					_timeCanvas.Hide();

					if (_condition is MovesFinishCondition moveCond)
					{
						limit = moveCond.Limit;
						_time.text = moveCond.Limit.Localize();
					}

					break;
			}

			var parsedToQR = _map.ToQRString(limit);
			_qr.texture = TextQRConverter.GenerateTexture(parsedToQR);

			_levelMap.BuildTileMap<TileView>(_map);
			_mapPreview.texture = _previewCamera.GetCurrentRender();
			_levelMap.Reset();
		}
	}
}