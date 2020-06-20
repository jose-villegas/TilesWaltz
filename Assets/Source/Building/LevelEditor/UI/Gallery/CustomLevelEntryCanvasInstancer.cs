using TilesWalk.Map.General;
using UnityEngine;
using Zenject;

namespace TilesWalk.Building.LevelEditor.UI.Gallery
{
	[RequireComponent(typeof(IMapProvider))]
	public class CustomLevelEntryCanvasInstancer : MonoBehaviour
	{
		[Inject] private DiContainer _container;
		[Inject] private MapProviderSolver _solver;
		[SerializeField] private CustomLevelEntryCanvas _entry;

		private void Start()
		{
			_solver.InstanceProvider(gameObject);

			if (_solver.Provider.Collection == null) return;

			foreach (var map in _solver.Provider.Collection.AvailableMaps)
			{
				var instance = _container.InstantiatePrefab(_entry.gameObject, transform);
				var canvas = instance.GetComponent<CustomLevelEntryCanvas>();
				canvas.name = map.Id;
				canvas.LevelRequest.Name.Value = map.Id;
			}
		}
	}
}
