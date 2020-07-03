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
		/// <param name="scale">Optional scale pulled from the CanvasScaler. Default to using Vector2.one.</param>
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

		public static Rect GetWorldSpaceRect(this RectTransform rt)
		{
			return GetWorldSpaceRect(rt, new Vector2(1f, 1f));
		}

		public static Rect GetScreenSpaceRect(this RectTransform rt)
		{
			Vector2 size = Vector2.Scale(rt.rect.size, rt.lossyScale);
			Rect rect = new Rect(rt.position.x, Screen.height - rt.position.y, size.x, size.y);
			rect.x -= (rt.pivot.x * size.x);
			rect.y -= ((1.0f - rt.pivot.y) * size.y);
			return rect;
		}

		public static Bounds GetRectTransformBounds(this RectTransform transform)
		{
			Vector3[] WorldCorners = new Vector3[4];
			transform.GetWorldCorners(WorldCorners);
			Bounds bounds = new Bounds(WorldCorners[0], Vector3.zero);
			for (int i = 1; i < 4; ++i)
			{
				bounds.Encapsulate(WorldCorners[i]);
			}
			return bounds;
		}
	}
}