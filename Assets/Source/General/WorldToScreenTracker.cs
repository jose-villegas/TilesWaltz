using UnityEngine;

namespace TilesWalk.General
{
	public class WorldToScreenTracker : MonoBehaviour
	{

		public Camera trackingCam;
		public Transform objectToTrack;
		public Vector3 pixelOffset;

		// Update is called once per frame
		void Update()
		{
			if (objectToTrack != null)
			{
				Vector3 screenPos = trackingCam.WorldToScreenPoint(objectToTrack.position);
				screenPos += pixelOffset;
				this.transform.position = screenPos;
			}
		}
	}
}
