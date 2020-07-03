using UnityEngine;
using Zenject;

namespace TilesWalk.Building.Gallery.UI
{
	public class ShowImportLevelCanvas : MonoBehaviour
	{
		[Inject] private ImportLevelCanvas _importCanvas;

		public void Show()
		{
			_importCanvas.gameObject.SetActive(true);
			_importCanvas.Show();
		}

		public void Hide()
		{
			_importCanvas.Hide();
			_importCanvas.gameObject.SetActive(false);
		}
	}
}