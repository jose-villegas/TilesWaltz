using TilesWalk.General.Patterns;
using TilesWalk.General.UI;
using UnityEngine;
using Zenject;

namespace TilesWalk.General.Camera
{
    /// <summary>
    /// Camera drag behavior, holding the pointer and moving around will update
    /// the camera position
    /// </summary>
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class CameraDrag : ObligatoryComponentBehaviour<UnityEngine.Camera>
    {
        /// <summary>
        /// Used to detect if the mouse is over any UI element
        /// </summary>
        [Inject] private CanvasHoverListener _canvasHover;

        /// <summary>
        /// This controls how 'fast' the drag movement works
        /// </summary>
        [SerializeField] private float _dragSpeed = 2;

        // <summary>
        /// Initial position when the pointer drag stars
        /// </summary>
        private Vector3 _dragOrigin;

        private void LateUpdate()
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