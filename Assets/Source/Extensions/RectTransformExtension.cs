using UnityEngine;

namespace TilesWalk.Extensions
{
	public static class RectTransformExtension
	{
		/// <summary>
		/// Converts RectTransform.rect local coordinates to world space
		/// Usage example RectTransformExt.GetWorldSpaceRect(myRect, Vector2.one);
		/// </summary>
		/// <returns>The world rect.</returns>
		/// <param name="rt">RectangleTransform we want to convert to world coordinates.</param>
		/// <param name="scale">Optional scale pulled from the CanvasScaler</param>
		public static Rect GetWorldSpaceRect(this RectTransform rt, Vector2 scale)
		{
			// Convert the rectangle to world corners and grab the top left
			Vector3[] corners = new Vector3[4];
			rt.GetWorldCorners(corners);
			Vector3 topLeft = corners[0];

			// Rescale the size appropriately based on the current Canvas scale
			Vector2 scaledSize = new Vector2(scale.x * rt.rect.size.x, scale.y * rt.rect.size.y);

			return new Rect(topLeft, scaledSize);
		}

		/// <summary>
		/// A rect for the <see cref="RectTransform"/> in screen space
		/// </summary>
		/// <param name="rt"></param>
		/// <returns></returns>
		public static Rect GetScreenSpaceRect(this RectTransform rt)
		{
			Vector2 size = Vector2.Scale(rt.rect.size, rt.lossyScale);
			Rect rect = new Rect(rt.position.x, Screen.height - rt.position.y, size.x, size.y);
			rect.x -= (rt.pivot.x * size.x);
			rect.y -= ((1.0f - rt.pivot.y) * size.y);
			return rect;
		}

		/// <summary>
		/// Gets a <see cref="Bounds"/> instance for the transform world corners
		/// </summary>
		/// <param name="transform"></param>
		/// <returns></returns>
		public static Bounds GetRectTransformBounds(this RectTransform transform)
		{
			Vector3[] worldCorners = new Vector3[4];
			transform.GetWorldCorners(worldCorners);
			Bounds bounds = new Bounds(worldCorners[0], Vector3.zero);

			for (int i = 1; i < 4; ++i)
			{
				bounds.Encapsulate(worldCorners[i]);
			}

			return bounds;
		}


		/// <summary>
		/// This methods will check is the rect transform is inside the screen or not
		/// </summary>
		/// <param name="rectTransform">Rect Trasform</param>
		/// <returns></returns>
		public static bool IsRectTransformInsideSreen(this RectTransform rectTransform)
		{
			var isInside = false;
			var corners = new Vector3[4];
			var visibleCorners = 0;

			rectTransform.GetWorldCorners(corners);
			var rect = new Rect(0, 0, Screen.width, Screen.height);

			foreach (Vector3 corner in corners)
			{
				if (rect.Contains(corner))
				{
					visibleCorners++;
				}
			}

			if (visibleCorners == 4)
			{
				isInside = true;
			}

			return isInside;
		}
	}
}