using System;
using System.Collections.Generic;
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
		[SerializeField] private CustomLevelEntryCanvas _entry;

		private Dictionary<string, CustomLevelEntryCanvas> _entries = new Dictionary<string, CustomLevelEntryCanvas>();

		private void Start()
		{
			_solver.InstanceProvider(gameObject);

			if (_solver.Provider.Collection?.AvailableMaps == null) return;

			foreach (var map in _solver.Provider.Collection.AvailableMaps)
			{
				var instance = _container.InstantiatePrefab(_entry.gameObject, transform);
				var canvas = instance.GetComponent<CustomLevelEntryCanvas>();
				canvas.name = map.Id;
				canvas.LevelRequest.RawName = map.Id;

				_entries.Add(map.Id, canvas);
			}

			_solver.Provider.Collection.OnLevelRemovedAsObservable().Subscribe(OnCollectionEntryRemoved).AddTo(this);
			_solver.Provider.Collection.OnNewLevelInsertAsObservable().Subscribe(OnCollectionEntryInserted).AddTo(this);
		}

		private void OnCollectionEntryInserted(LevelMap map)
		{
			// this meants the entry was replaced
			if (_entries.TryGetValue(map.Id, out var canvas))
			{
				Destroy(canvas.gameObject);
			}

			var instance = _container.InstantiatePrefab(_entry.gameObject, transform);
			var newCanvas = instance.GetComponent<CustomLevelEntryCanvas>();
			newCanvas.name = map.Id;
			newCanvas.LevelRequest.RawName = map.Id;

			_entries[map.Id] = newCanvas;
		}

		private void OnCollectionEntryRemoved(LevelMap map)
		{
			if (_entries.TryGetValue(map.Id, out var canvas))
			{
				Destroy(canvas.gameObject);
				_entries.Remove(map.Id);
			}
		}
	}
}