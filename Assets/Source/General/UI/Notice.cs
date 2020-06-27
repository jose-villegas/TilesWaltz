using System;
using TilesWalk.Gameplay.Display;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TilesWalk.General.UI
{
	public class Notice : CanvasGroupBehaviour
	{
		[Inject] private GameTileColorsConfiguration _colorsConfiguration;

		[SerializeField] private TextMeshProUGUI _content;
		[SerializeField] private Image _panel;
		[SerializeField] private Image _message;
		[SerializeField] private Image _warning;
		[SerializeField] private Image _error;

		private void Awake()
		{
			Hide();
		}

		public Notice Configure(string message, NoticePriority priority = NoticePriority.Warning,
			GameColor panel = GameColor.Warn, GameColor text = GameColor.Blank)
		{
			_content.text = message;
			_panel.color = _colorsConfiguration[panel];
			_content.color = _colorsConfiguration[text];

			_message.enabled = false;
			_warning.enabled = false;
			_error.enabled = false;

			switch (priority)
			{
				case NoticePriority.Message:
					_message.enabled = true;
					break;
				case NoticePriority.Warning:
					_warning.enabled = true;
					break;
				case NoticePriority.Error:
					_error.enabled = true;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(priority), priority, null);
			}

			return this;
		}

		public void Show(float time)
		{
			Show();
			Observable.Timer(TimeSpan.FromSeconds(time)).Subscribe(_ => { }, () => Hide()).AddTo(this);
		}
	}
}