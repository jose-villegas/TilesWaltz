using TilesWalk.General.Patterns;
using TilesWalk.Map.Bridge;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TilesWalk.Map.UI
{
	[RequireComponent(typeof(Button))]
	public class ClearBridgeButton : ObligatoryComponentBehaviour<Button>
	{
		[Inject] private LevelBridge _bridge;

		private void Awake()
		{
			var button = Component;

			button.onClick.AddListener(ClearBridgePayload);
		}

		private void ClearBridgePayload()
		{
			_bridge.Payload = null;
		}
	}
}