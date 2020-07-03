using System;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Condition;
using TilesWalk.Map.General;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Building.Gallery.UI
{
	[RequireComponent(typeof(IMapProvider))]
	public class CustomLevelEntryCanvasInstancer : MonoBehaviour
	{
		[Inject] private DiContainer _container;
		[Inject] private MapProviderSolver _solver;
		[Inject(Optional = true)] private ImportLevelCanvas _importCanvas;
		[SerializeField] private CustomLevelEntryCanvas _entry;

		private void Start()
		{
			_solver.InstanceProvider(gameObject);

			if (_solver.Provider.Collection?.AvailableMaps == null) return;

			foreach (var map in _solver.Provider.Collection.AvailableMaps)
			{
				var instance = _container.InstantiatePrefab(_entry.gameObject, transform);
				var canvas = instance.GetComponent<CustomLevelEntryCanvas>();
				canvas.name = map.Id;
				canvas.LevelRequest.Name.Value = map.Id;
			}

			if (_importCanvas != null)
			{
				_importCanvas.OnNewLevelImportedAsObservable().Subscribe(OnNewLevelImported).AddTo(this);
			}
		}

		private void OnNewLevelImported(Tuple<LevelMap, MapFinishCondition> map)
		{
			var instance = _container.InstantiatePrefab(_entry.gameObject, transform);
			var canvas = instance.GetComponent<CustomLevelEntryCanvas>();
			canvas.name = map.Item1.Id;
			canvas.LevelRequest.Name.Value = map.Item1.Id;
		}
	}
}