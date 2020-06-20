using System;
using UnityEngine;

namespace TilesWalk.Map.General
{
	[Serializable]
	public class MapProviderSolver
	{
		[SerializeField] private Provider _mapProvider;

		public IMapProvider Provider
		{
			get
			{
				if (_provider == null)
				{
					_provider = _reference.GetComponent<IMapProvider>();
				}

				return _provider;
			}
		}

		private IMapProvider _provider;
		private GameObject _reference;

		public MapProviderSolver(GameObject reference)
		{
			_reference = reference;
			InstanceProvider();
		}

		public void InstanceProvider()
		{
			if (Provider == null)
			{
				switch (_mapProvider)
				{
					case General.Provider.UserMaps:
						_provider = _reference.AddComponent<UserLevelMapsProvider>();
						break;
					case General.Provider.ImportedMaps:
						// todo: add imported maps
						break;
					case General.Provider.GameMaps:
						_provider = _reference.AddComponent<GameLevelMapsProvider>();
						break;
				}
			}
		}
	}
}