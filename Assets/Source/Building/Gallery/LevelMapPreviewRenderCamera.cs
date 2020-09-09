using TilesWalk.General.Patterns;
using UnityEngine;

namespace TilesWalk.Building.Gallery
{
    [RequireComponent(typeof(Camera))]
    public class LevelMapPreviewRenderCamera : ObligatoryComponentBehaviour<Camera>
    {
        private RenderTexture _previewRenderTexture;

        /// <summary>
        /// Gets the current rendered frame for the preview camera
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public Texture2D GetSnapshotRender(int width = 256, int height = 256)
        {
            Rect rect = new Rect(0, 0, width, height);
            RenderTexture renderTexture = new RenderTexture(width, height, 16);
            Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);

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

        public RenderTexture BeginRendering(int width = 256, int height = 256)
        {
            if (_previewRenderTexture == null)
            {
                _previewRenderTexture = new RenderTexture(width, height, 16);
            }

            Component.targetTexture = _previewRenderTexture;
            Component.enabled = true;

            return _previewRenderTexture;
        }

        public void EndRendering()
        {
            Component.targetTexture = null;
            Component.enabled = false;
        }
    }
}