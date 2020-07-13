using Hellmade.Sound;
using System;
using TilesWalk.General.UI;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TilesWalk.Gameplay.UI
{
	public class GameOptionsCanvas : CanvasGroupBehaviour
    {
        [Inject] private GameplaySetting _setting;

		[SerializeField] private Button _close;
		[SerializeField] private Button _quit;
        [SerializeField] private Toggle _guide;
		[SerializeField] private Toggle _music;
		[SerializeField] private Toggle _effects;

		private void Awake()
		{
			Hide();
			
			OnMusicToggle(_music.isOn = _setting.Music);
			OnEffectsToggle(_effects.isOn = _setting.Sounds);

			_guide.onValueChanged.AddListener(OnGuidesToggle);
			_music.onValueChanged.AddListener(OnMusicToggle);
			_effects.onValueChanged.AddListener(OnEffectsToggle);
		}

        private void OnGuidesToggle(bool value)
        {
            _setting.ShowGuides = value;
		}

        private void OnEffectsToggle(bool value)
		{
			EazySoundManager.GlobalSoundsVolume = value ? 1f : 0f;
			EazySoundManager.GlobalUISoundsVolume = value ? 1f : 0f;
            _setting.Sounds = value;
        }

		private void OnMusicToggle(bool value)
		{
			EazySoundManager.GlobalMusicVolume = value ? 1f : 0f;
            _setting.Music = value;
		}

		public void ApplicationQuit()
		{
			Application.Quit();
		}
	}
}