using System;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Level;
using TilesWalk.Map.General;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Building.Gallery.UI
{
	public class DeactivateOnCustomMapLimit : MonoBehaviour
	{
		[Inject] private MapProviderSolver _solver;
		[Inject] private CustomLevelsConfiguration _customLevelsConfiguration;

		private int _maximum;
		private int _current;

		private void Awake()
		{
			_solver.InstanceProvider(gameObject);

			if (_solver.Provider.Collection == null || _solver.Provider.Collection.AvailableMaps == null) return;

			_maximum = _solver.Provider.MaximumLevels;
			_current = _solver.Provider.Collection.AvailableMaps.Count;

			_solver.Provider.Collection.OnLevelRemovedAsObservable().Subscribe(OnCollectionUpdated).AddTo(this);
			_solver.Provider.Collection.OnNewLevelInsertAsObservable().Subscribe(OnCollectionUpdated).AddTo(this);

			gameObject.SetActive(_current < _maximum);
		}

		private void OnCollectionUpdated(LevelMap level)
		{
			_current = _solver.Provider.Collection.AvailableMaps.Count;
			gameObject.SetActive(_current < _maximum);
		}
	}
}