using UnityEngine;
using UnityEngine.EventSystems;

namespace TilesWalk.General.UI
{
	[RequireComponent(typeof(Canvas))]
	public class CanvasHoverListener : MonoBehaviour
	{
		public bool IsUIOverride { get; private set; }

		void Update()
		{
			IsUIOverride = EventSystem.current.IsPointerOverGameObject();
		}
	}
}
