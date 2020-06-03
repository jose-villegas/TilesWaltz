using UnityEngine;

namespace TilesWalk.Navigation.UI
{
	public class ObligatoryComponentBehaviour<T> : MonoBehaviour where T : Component
	{
		private T _component;

		public T Component
		{
			get
			{
				if (_component == null)
				{
					_component = GetComponent<T>();

					if (_component == null)

					{
						_component = this.gameObject.AddComponent<T>();
					}
				}

				return _component;
			}
		}
	}
}