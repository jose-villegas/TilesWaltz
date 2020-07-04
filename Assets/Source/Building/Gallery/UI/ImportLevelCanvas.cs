using System;
using System.IO.MemoryMappedFiles;
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
using UnityEngine.Android;
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
		[Inject] private Notice _notice;
		[Inject] private Confirmation _confirmation;

		[SerializeField] private Animator _animator;
		[SerializeField] private RawImage _cameraRenderer;

		[Header("Content")] [SerializeField] private TextMeshProUGUI _title;
		[SerializeField] private TextMeshProUGUI _points;

		[SerializeField] private RawImage _mapPreview;
		[SerializeField] private RawImage _qr;

		[SerializeField] private CanvasGroupBehaviour _timeCanvas;
		[SerializeField] private TextMeshProUGUI _time;
		[SerializeField] private CanvasGroupBehaviour _movesCanvas;
		[SerializeField] private TextMeshProUGUI _moves;

		[SerializeField] private TMP_InputField _codeInput;
		[SerializeField] private Toggle _qrToggle;
		[SerializeField] private Toggle _codeToggle;

		[Header("Actions")] [SerializeField] private Button _add;

		private WebCamTexture _cameraTexture;

		private LevelMap _map;
		private MapFinishCondition _condition;
		private bool _isMapRead;
		private bool _cameraAvailable;
		private bool _askingPermission;
		private IDisposable _qrCheck = null;

		private Subject<Tuple<LevelMap, MapFinishCondition>> _onNewLevelImported;

		private void Awake()
		{
			_add.interactable = false;
			_qrToggle.onValueChanged.AsObservable().Subscribe(OnQRToggle).AddTo(this);
			_codeInput.onEndEdit.AsObservable().Subscribe(OnCodeEntered).AddTo(this);
			_add.onClick.AsObservable().Subscribe(OnSaveClick).AddTo(this);
		}

		private void OnSaveClick(Unit obj)
		{
			if (_isMapRead && _map != null && _condition != null)
			{
				if (_solver.Provider.Collection.Exist(_map.Id))
				{
					_confirmation.Configure("There is another map with the same name, replace?", () =>
					{
						_solver.Provider.Collection.Insert(_map, _condition);
						_onNewLevelImported?.OnNext(new Tuple<LevelMap, MapFinishCondition>(_map, _condition));
						gameObject.SetActive(false);
					}).Show();
				}
				else
				{
					_solver.Provider.Collection.Insert(_map, _condition);
					_onNewLevelImported?.OnNext(new Tuple<LevelMap, MapFinishCondition>(_map, _condition));
					gameObject.SetActive(false);
				}
			}
		}

		public IObservable<Tuple<LevelMap, MapFinishCondition>> OnNewLevelImportedAsObservable()
		{
			return _onNewLevelImported = _onNewLevelImported == null
				? new Subject<Tuple<LevelMap, MapFinishCondition>>()
				: _onNewLevelImported;
		}

		private void OnDestroy()
		{
			_onNewLevelImported?.OnCompleted();
		}

		private void OnCodeEntered(string code)
		{
			if (_isMapRead) return;

			if (!_codeToggle.isOn) return;

			try
			{
				LevelMap.FromQRString(code, out _map, out _condition);
				_animator.SetTrigger("ScanningDone");
				_isMapRead = true;
				_add.interactable = true;
				_cameraTexture.Stop();
				_codeInput.readOnly = true;

				UpdateCanvas();
			}
			catch (Exception e)
			{
				Debug.LogWarning(e.Message);
			}
		}

		private void OnQRToggle(bool val)
		{
			if (_isMapRead) return;

			if (!_cameraAvailable && val)
			{
				InitializeCamera();
				_cameraTexture.Play();

				if (_qrCheck == null)
				{
					_qrCheck = Observable.Interval(TimeSpan.FromMilliseconds(250)).Subscribe(_ => ReadQR());
				}
			}
			else if (_cameraAvailable && val)
			{
				_cameraTexture.Play();
				if (_qrCheck == null)
				{
					_qrCheck = Observable.Interval(TimeSpan.FromMilliseconds(250)).Subscribe(_ => ReadQR());
				}
			}
			else if (!val)
			{
				_cameraTexture.Stop();
				_qrCheck?.Dispose();
				_qrCheck = null;
			}
		}

		private void OnDisable()
		{
			_qrCheck?.Dispose();
			_qrCheck = null;

			if (_cameraTexture != null)
			{
				_cameraTexture.Stop();
			}

			_isMapRead = false;
			_codeInput.readOnly = false;
			_animator.SetTrigger("ScanningMode");
		}

		private void InitializeCamera()
		{
			WebCamDevice[] devices = WebCamTexture.devices;

			if (devices.Length == 0)
			{
				return;
			}

			_cameraTexture = new WebCamTexture(Screen.width, Screen.height);

			if (_cameraTexture == null)
			{
				return;
			}

			_cameraRenderer.texture = _cameraTexture;
			_cameraAvailable = true;
		}

		public override void Show()
		{
			if (_qrToggle.isOn)
			{
#if UNITY_EDITOR
				if (!_cameraAvailable) InitializeCamera();

				base.Show();
#elif PLATFORM_ANDROID
			if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
			{
				_askingPermission = true;
				Permission.RequestUserPermission(Permission.Camera);
			}
			else
			{
				base.Show();

				if (!_cameraAvailable) InitializeCamera();
			}
#endif
				if (_qrCheck == null)
				{
					// check qr data
					_qrCheck = Observable.Interval(TimeSpan.FromMilliseconds(250)).Subscribe(_ => ReadQR());
				}

				if (_cameraAvailable)
				{
					_cameraTexture.Play();
				}
			}
		}

		private void OnApplicationFocus(bool val)
		{
			if (_qrToggle.isOn)
			{
				if (_askingPermission && val)
				{
					if (Permission.HasUserAuthorizedPermission(Permission.Camera))
					{
						if (!_cameraAvailable) InitializeCamera();

						_notice.Configure("Close and reopen this popup if not rendering.")
							.Show(2f);

						base.Show();
					}
					else
					{
						_notice.Configure("Needs camera permission to scan new maps from QR codes.",
								NoticePriority.Error)
							.Show(3f);
					}
				}
			}
		}

		private void Update()
		{
			if (!_cameraAvailable || !_qrToggle.isOn)
				return;

			float scaleY = _cameraTexture.videoVerticallyMirrored ? -1f : 1f; // Find if the camera is mirrored or not
			_cameraRenderer.rectTransform.localScale = new Vector3(1f, scaleY, 1f); // Swap the mirrored camera

			int orient = -_cameraTexture.videoRotationAngle;
			_cameraRenderer.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
		}

		private void ReadQR()
		{
			if (_isMapRead) return;

			try
			{
				if (!_cameraTexture.isPlaying) return;

				IBarcodeReader barcodeReader = new BarcodeReader();

				// decode the current frame
				var result = barcodeReader.Decode(_cameraTexture.GetPixels32(), _cameraTexture.width,
					_cameraTexture.height);

				if (result != null)
				{
					LevelMap.FromQRString(result.Text, out _map, out _condition);
					_animator.SetTrigger("ScanningDone");
					_isMapRead = true;
					_add.interactable = true;
					_cameraTexture.Stop();

					UpdateCanvas();
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
						_moves.text = moveCond.Limit.Localize();
					}

					break;
			}

			var parsedToQR = _map.ToQRString(limit);
			_qr.texture = TextQRConverter.GenerateTexture(parsedToQR);

			_levelMap.BuildTileMap<TileView>(_map);
			_mapPreview.texture = _previewCamera.GetCurrentRender(512, 512);
			_levelMap.Reset();
		}
	}
}