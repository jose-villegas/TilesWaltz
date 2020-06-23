using TilesWalk.General.Patterns;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TilesWalk.Audio.UI
{
	[RequireComponent(typeof(Button))]
	public class ButtonClickSound : ObligatoryComponentBehaviour<Button>
	{
		[Inject] private GameAudioCollection _audioCollection;
		[SerializeField] private string identifier = "Click";

		private void Awake()
		{
			Component.onClick.AddListener(() => _audioCollection.Play(GameAudioType.UI, identifier));
		}
	}
}