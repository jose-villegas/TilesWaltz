using UnityEngine;

namespace TilesWalk.General.Movement
{
	public class CameraFacingBillboard : MonoBehaviour
	{
		// Orient the camera after all movement is completed this frame to avoid jittering
		void LateUpdate()
		{
			transform.LookAt(transform.position + UnityEngine.Camera.main.transform.rotation * Vector3.forward,
				UnityEngine.Camera.main.transform.rotation * Vector3.up);
		}
	}
}