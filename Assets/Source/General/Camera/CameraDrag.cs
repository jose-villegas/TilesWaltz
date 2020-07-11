using TilesWalk.General.Patterns;
using TilesWalk.General.UI;
using UnityEngine;
using Zenject;

namespace TilesWalk.General.Camera
{
	[RequireComponent(typeof(UnityEngine.Camera))]
	public class CameraDrag : ObligatoryComponentBehaviour<UnityEngine.Camera>
	{
        [Inject] private CanvasHoverListener _canvasHover;

		[SerializeField] private float _dragSpeed = 2;

		private Vector3 _dragOrigin;

		void LateUpdate()
		{
            if (_canvasHover.IsUIOverride) return;

			if (Input.GetMouseButtonDown(0))
			{
				_dragOrigin = Input.mousePosition;
				return;
			}

			if (!Input.GetMouseButton(0)) return;

			Vector3 pos = Component.ScreenToViewportPoint(Input.mousePosition - _dragOrigin);
			Vector3 move = new Vector3(pos.x * _dragSpeed, pos.y * _dragSpeed, 0);

			transform.Translate(move, Space.Self);
		}
	}
}
