using TilesWalk.Map.General;
using UnityEngine;

namespace TilesWalk.Building.LevelEditor.UI.Gallery
{
	[RequireComponent(typeof(IMapProvider))]
	public class CustomLevelEntryCanvasInstancer : MonoBehaviour
	{
		[SerializeField] private MapProviderSolver _solver;
		[SerializeField] private CustomLevelEntryCanvas _entry;

		private void Start()
		{
			if (_solver == null) _solver = new MapProviderSolver(gameObject);

			_solver.InstanceProvider();

			foreach (var map in _solver.Provider.AvailableMaps)
			{
				var canvas = Instantiate(_entry);
				canvas.name = map.Id;
				canvas.LevelRequest.Name.Value = map.Id;
			}
		}
	}
}
