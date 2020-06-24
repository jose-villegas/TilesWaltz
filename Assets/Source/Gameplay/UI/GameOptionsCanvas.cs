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

			_music.onValueChanged.AddListener(value => EazySoundManager.GlobalMusicVolume = value ? 1f : 0f);
			_effects.onValueChanged.AddListener(value => EazySoundManager.GlobalSoundsVolume = value ? 1f : 0f);
			_effects.onValueChanged.AddListener(value => EazySoundManager.GlobalUISoundsVolume = value ? 1f : 0f);
		}
	}
}