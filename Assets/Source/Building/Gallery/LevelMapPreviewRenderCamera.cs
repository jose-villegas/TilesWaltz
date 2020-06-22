using TilesWalk.General.Patterns;
using UnityEngine;

namespace TilesWalk.Building.Gallery
{
	[RequireComponent(typeof(Camera))]
	public class LevelMapPreviewRenderCamera : ObligatoryComponentBehaviour<Camera>
	{
		public Texture2D GetCurrentRender(int width = 256, int height = 256)
		{
			Rect rect = new Rect(0, 0, width, height);
			RenderTexture renderTexture = new RenderTexture(width, height, 24);
			Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGBA32, false);

			Component.targetTexture = renderTexture;
			Component.Render();

			RenderTexture.active = renderTexture;
			screenShot.ReadPixels(rect, 0, 0);
			screenShot.Apply();

			Component.targetTexture = null;
			RenderTexture.active = null;

			Destroy(renderTexture);
			renderTexture = null;
			return screenShot;
		}
	}
}