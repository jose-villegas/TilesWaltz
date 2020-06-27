using TilesWalk.General.Patterns;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TilesWalk.Audio.UI
{
	[RequireComponent(typeof(Toggle))]
	public class ToggleClickSound : ObligatoryComponentBehaviour<Toggle>
	{
		[Inject] private GameAudioCollection _audioCollection;
		[SerializeField] private string identifier = "Click";

		private void Awake()
		{
			Component.onValueChanged.AddListener(val => _audioCollection.Play(GameAudioType.UI, identifier));
		}
	}
}