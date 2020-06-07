using TilesWalk.Navigation.UI;
using UnityEngine;

namespace TilesWalk.General.UI
{
	[RequireComponent(typeof(CanvasGroup))]
	public class CanvasGroupBehaviour : ObligatoryComponentBehaviour<CanvasGroup>
	{
		public bool IsVisible => Component.alpha > 0;

		public void Hide()
		{
			Component.alpha = 0;
			Component.interactable = false;
		}

		public void Show()
		{
			Component.alpha = 1;
			Component.interactable = true;
		}
	}
}
