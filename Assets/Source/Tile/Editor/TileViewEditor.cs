using NaughtyAttributes.Editor;
using UnityEditor;
using UnityEngine;

namespace TilesWalk.Tile.Editor
{
	[CustomEditor(typeof(TileView))]
	public class TileViewEditor : NaughtyInspector
	{
		[DrawGizmo(GizmoType.Selected)]
		private static void RenderCustomGizmoSelected(Transform objectTransform, GizmoType gizmoType)
		{
			DrawHandles(objectTransform);
		}

		[DrawGizmo(GizmoType.NonSelected)]
		private static void RenderCustomGizmoNonSelected(Transform objectTransform, GizmoType gizmoType)
		{
			DrawHandles(objectTransform);
		}

		private static void DrawHandles(Transform objectTransform)
		{
			var t = objectTransform.GetComponent<TileView>();

			if (t == null) return;


			Handles.color = Handles.xAxisColor;
			var rotation = Quaternion.LookRotation(objectTransform.right, objectTransform.forward);
			Handles.ArrowHandleCap(0, t.transform.position, rotation, 1, EventType.Repaint);

			Handles.color = Handles.yAxisColor;
			rotation = Quaternion.LookRotation(objectTransform.up, objectTransform.right);
			Handles.ArrowHandleCap(0, t.transform.position, rotation, 1, EventType.Repaint);

			Handles.color = Handles.zAxisColor;
			rotation = Quaternion.LookRotation(objectTransform.forward, objectTransform.up);
			Handles.ArrowHandleCap(0, t.transform.position, rotation, 1, EventType.Repaint);
		}
	}
}