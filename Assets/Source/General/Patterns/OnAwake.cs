using UnityEngine;
using UnityEngine.Events;

namespace TilesWalk.General.Patterns
{
	public class OnAwake : MonoBehaviour
	{
		[SerializeField] private UnityEvent _onAwake;

		private void Awake()
		{
			_onAwake?.Invoke();
		}
	}
}
