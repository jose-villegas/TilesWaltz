using System;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Level;
using TilesWalk.General.Patterns;
using TilesWalk.Map.General;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Building.Gallery.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class CustomMapsCounter : ObligatoryComponentBehaviour<TextMeshProUGUI>
	{
		[Inject] private MapProviderSolver _solver;
		[Inject] private CustomLevelsConfiguration _customLevelsConfiguration;

		private int _maximum;
		private int _current;

		private void Awake()
		{
			_solver.InstanceProvider(gameObject);

			_maximum = _solver.Provider.MaximumLevels;
			_current = _solver.Provider.Collection.AvailableMaps?.Count ?? 0;
			Component.text = $"({_current}/{_maximum})";

			_solver.Provider.Collection.OnLevelRemovedAsObservable().Subscribe(OnCollectionUpdated).AddTo(this);
			_solver.Provider.Collection.OnNewLevelInsertAsObservable().Subscribe(OnCollectionUpdated).AddTo(this);
		}

		private void OnCollectionUpdated(LevelMap level)
		{
			_current = _solver.Provider.Collection.AvailableMaps.Count;
			Component.text = $"({_current}/{_maximum})";
		}
	}
}
