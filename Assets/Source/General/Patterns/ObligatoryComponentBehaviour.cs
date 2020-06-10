using UniRx.Triggers;
using UnityEngine;

namespace TilesWalk.General.Patterns
{
	public class ObligatoryComponentBehaviour<T> : ObservableTriggerBase where T : Component
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

		protected override void RaiseOnCompletedOnDestroy()
		{
		}
	}
}