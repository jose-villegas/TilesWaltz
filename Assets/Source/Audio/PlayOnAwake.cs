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
			_audioCollection.Play(_type, identifier, loop, persist);
		}
	}
}