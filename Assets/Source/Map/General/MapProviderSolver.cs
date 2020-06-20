using System;
using UnityEngine;
using Zenject;

namespace TilesWalk.Map.General
{
	[Serializable]
	public class MapProviderSolver : MonoBehaviour
	{
		[Inject] private DiContainer _container;
		[SerializeField] private Provider _mapProvider;

		public IMapProvider Provider
		{
			get
			{
				if (_provider == null && _reference != null)
				{
					_provider = _reference.GetComponent<IMapProvider>();
				}

				return _provider;
			}
		}

		private IMapProvider _provider;
		private GameObject _reference;

		public void InstanceProvider(GameObject reference)
		{
			_reference = reference;


			if (Provider == null)
			{
				switch (_mapProvider)
				{
					case General.Provider.UserMaps:
						_provider = _container.InstantiateComponent(typeof(UserLevelMapsProvider), _reference) as UserLevelMapsProvider;
						break;
					case General.Provider.ImportedMaps:
						// todo: add imported maps
						break;
					case General.Provider.GameMaps:
						_provider = _container.InstantiateComponent(typeof(GameLevelMapsProvider), _reference) as GameLevelMapsProvider;
						break;
				}
			}
		}
	}
}