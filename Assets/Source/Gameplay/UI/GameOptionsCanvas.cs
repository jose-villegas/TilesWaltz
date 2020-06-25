using Hellmade.Sound;
using TilesWalk.General.UI;
using UnityEngine;
using UnityEngine.UI;

namespace TilesWalk.Gameplay.UI
{
	public class GameOptionsCanvas : CanvasGroupBehaviour
	{
		[SerializeField] private Button _close;
		[SerializeField] private Button _quit;
		[SerializeField] private Toggle _music;
		[SerializeField] private Toggle _effects;

		private void Awake()
		{
			Hide();
			
			var musicPreference = bool.Parse(PlayerPrefs.GetString("Music", true.ToString()));
			var effectsPreference = bool.Parse(PlayerPrefs.GetString("Effects", true.ToString()));

			OnMusicToggle(_music.isOn = musicPreference);
			OnEffectsToggle(_effects.isOn = effectsPreference);

			_music.onValueChanged.AddListener(OnMusicToggle);
			_effects.onValueChanged.AddListener(OnEffectsToggle);
		}

		private void OnEffectsToggle(bool value)
		{
			EazySoundManager.GlobalSoundsVolume = value ? 1f : 0f;
			EazySoundManager.GlobalUISoundsVolume = value ? 1f : 0f;
			PlayerPrefs.SetString("Effects", value.ToString());
			PlayerPrefs.Save();
		}

		private void OnMusicToggle(bool value)
		{
			EazySoundManager.GlobalMusicVolume = value ? 1f : 0f;
			PlayerPrefs.SetString("Music", value.ToString());
			PlayerPrefs.Save();
		}

		public void ApplicationQuit()
		{
			Application.Quit();
		}
	}
}