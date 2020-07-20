using Hellmade.Sound;
using UnityEngine;
using Zenject;

namespace TilesWalk.Audio
{
    public class PlayOnAwake : MonoBehaviour
    {
        [Inject] private GameAudioCollection _audioCollection;
        [SerializeField] private GameAudioType _type;
        [SerializeField] private string identifier;
        [SerializeField] private bool loop;
        [SerializeField] private bool persist;

        private void Awake()
        {
            if (persist)
            {
                var clip = _audioCollection.GetAudioClip(_type, identifier);

                var audio = EazySoundManager.GetAudio(clip);

                if (audio == null || !audio.IsPlaying)
                {
                    _audioCollection.Play(_type, identifier, loop, persist);
                }
            }
            else
            {
                _audioCollection.Play(_type, identifier, loop, persist);
            }
        }
    }
}