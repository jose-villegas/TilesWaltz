using TilesWalk.General.Patterns;
using UnityEngine;

namespace TilesWalk.General.Camera
{
	[RequireComponent(typeof(UnityEngine.Camera))]
	public class CameraDrag : ObligatoryComponentBehaviour<UnityEngine.Camera>
	{
		public float dragSpeed = 2;
		private Vector3 dragOrigin;

		void LateUpdate()
		{
			if (Input.GetMouseButtonDown(0))
			{
				dragOrigin = Input.mousePosition;
				return;
			}

			if (!Input.GetMouseButton(0)) return;

			Vector3 pos = Component.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
			Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0);

			transform.Translate(move, Space.Self);
		}
	}
}
