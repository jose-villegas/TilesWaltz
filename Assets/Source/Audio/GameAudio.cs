using System;
using Hellmade.Sound;
using UnityEngine;

namespace TilesWalk.Audio
{
	[Serializable]
	public class GameAudio
	{
		[SerializeField] private string _identifier;
		[SerializeField] private AudioClip _clip;
		[SerializeField] private Hellmade.Sound.Audio _audio;

		public string Identifier => _identifier;

		public void PlayMusic(float volume = 1f, bool loop = false, bool persist = false, float fadeInSeconds = 1f,
			float fadeOutSeconds = 1f, float currentMusicfadeOutSeconds = -1f, Transform sourceTransform = null)
		{
			int audioID = EazySoundManager.PlayMusic(_clip, volume, loop, persist, fadeInSeconds, fadeOutSeconds,
				currentMusicfadeOutSeconds, sourceTransform);
			_audio = EazySoundManager.GetAudio(audioID);
		}

		public void PlaySound(float volume = 1f, bool loop = false, Transform sourceTransform = null)
		{
			int audioID = EazySoundManager.PlaySound(_clip, volume, loop, sourceTransform);
			_audio = EazySoundManager.GetAudio(audioID);
		}

		public void PlayUISound(float volume = 1f)
		{
			int audioID = EazySoundManager.PlayUISound(_clip, volume);
			_audio = EazySoundManager.GetAudio(audioID);
		}
	}
}