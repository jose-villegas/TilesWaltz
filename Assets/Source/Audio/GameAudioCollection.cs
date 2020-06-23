using System;
using System.Collections.Generic;
using UnityEngine;

namespace TilesWalk.Audio
{
	[Serializable]
	public class GameAudioCollection
	{
		[SerializeField] private List<GameAudio> _gameSounds = new List<GameAudio>();
		[SerializeField] private List<GameAudio> _gameUISounds = new List<GameAudio>();
		[SerializeField] private List<GameAudio> _gameMusic = new List<GameAudio>();

		/// <summary>
		/// Plays an audio asset
		/// </summary>
		/// <param name="type">The audio type</param>
		/// <param name="identifier">An unique identifier per type</param>
		/// <param name="loop">
		/// Loop the audio, valid only for <see cref="GameAudioType.Sound"/>
		/// and <see cref="GameAudioType.Music"/>
		/// </param>
		/// <param name="persist">
		/// Persist the audio component, valid only for <see cref="GameAudioType.Music"/>
		/// </param>
		/// <param name="fadeInSeconds">
		/// Fade in effect entry, valid only for <see cref="GameAudioType.Music"/>
		/// </param>
		/// <param name="fadeOutSeconds">
		/// Fade out effect exit, valid only for <see cref="GameAudioType.Music"/>
		/// </param>
		/// <param name="currentMusicfadeOutSeconds">
		/// Current fadeout, valid only for <see cref="GameAudioType.Music"/>
		/// </param>
		/// <param name="sourceTransform">
		///	If given this enables 3D aware audio, valid only for <see cref="GameAudioType.Sound"/>
		/// and <see cref="GameAudioType.Music"/>
		/// </param>
		public void Play(GameAudioType type, string identifier, bool loop = false,
			bool persist = false, float fadeInSeconds = 1f, float fadeOutSeconds = 1f,
			float currentMusicfadeOutSeconds = -1f, Transform sourceTransform = null)
		{
			var indexOf = -1;

			switch (type)
			{
				case GameAudioType.Music:
					indexOf = _gameMusic.FindIndex(x => x.Identifier == identifier);
					if (indexOf >= 0)
					{
						_gameMusic[indexOf].PlayMusic(loop, persist, fadeInSeconds, fadeOutSeconds,
							currentMusicfadeOutSeconds, sourceTransform);
					}

					break;
				case GameAudioType.Sound:
					indexOf = _gameSounds.FindIndex(x => x.Identifier == identifier);
					if (indexOf >= 0)
					{
						_gameSounds[indexOf].PlaySound(loop, sourceTransform);
					}

					break;
				case GameAudioType.UI:
					indexOf = _gameUISounds.FindIndex(x => x.Identifier == identifier);
					if (indexOf >= 0)
					{
						_gameUISounds[indexOf].PlayUISound();
					}

					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}
		}
	}
}