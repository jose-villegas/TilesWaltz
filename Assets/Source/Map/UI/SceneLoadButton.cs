using NaughtyAttributes;
using TilesWalk.General.Patterns;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TilesWalk.Navigation.UI
{
	[RequireComponent(typeof(Button))]
	public class SceneLoadButton : ObligatoryComponentBehaviour<Button>
	{
		[Scene] public string SceneToLoad;
		public LoadSceneMode Mode;

		private void Awake()
		{
			var button = Component;

			button.onClick.AddListener(LoadScene);
		}

		private void LoadScene()
		{
			SceneManager.LoadScene(SceneToLoad, Mode);
		}
	}
}