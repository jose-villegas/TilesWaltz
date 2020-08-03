using System.Collections;
using System.Collections.Generic;
using TilesWalk.Building.Level;
using TilesWalk.Map.General;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Building.Gallery.UI
{
    /// <summary>
    /// This class instances entries for the gallery view, it handles user
    /// custom maps and imported custom maps
    /// </summary>
    [RequireComponent(typeof(IMapProvider))]
    public class CustomLevelEntryCanvasInstancer : MonoBehaviour
    {
        [Inject] private DiContainer _container;
        [Inject] private MapProviderSolver _solver;
        [SerializeField] private CustomLevelEntryCanvas _entry;

        private Dictionary<string, CustomLevelEntryCanvas> _entries = new Dictionary<string, CustomLevelEntryCanvas>();

        private IEnumerator Start()
        {
            _solver.InstanceProvider(gameObject);


            _solver.Provider.Collection.OnLevelRemovedAsObservable().Subscribe(OnCollectionEntryRemoved).AddTo(this);
            _solver.Provider.Collection.OnNewLevelInsertAsObservable().Subscribe(OnCollectionEntryInserted).AddTo(this);

            if (_solver.Provider.Collection.AvailableMaps == null) yield break;

            var instances = new List<CustomLevelEntryCanvas>();

            foreach (var map in _solver.Provider.Collection.AvailableMaps)
            {
                var instance = _container.InstantiatePrefab(_entry.gameObject, transform);
                var canvas = instance.GetComponent<CustomLevelEntryCanvas>();
                canvas.name = map.Id;
                canvas.LevelRequest.RawName = map.Id;

                instances.Add(canvas);
                _entries.Add(map.Id, canvas);
            }

            foreach (var canvas in instances)
            {
                canvas.RefreshMapPreview();
                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        /// When the map collection is modified through insertion
        /// </summary>
        /// <param name="map"></param>
        private void OnCollectionEntryInserted(LevelMap map)
        {
            // this means the entry was replaced
            if (_entries.TryGetValue(map.Id, out var canvas))
            {
                Destroy(canvas.gameObject);
            }

            var instance = _container.InstantiatePrefab(_entry.gameObject, transform);
            var newCanvas = instance.GetComponent<CustomLevelEntryCanvas>();
            newCanvas.name = map.Id;
            newCanvas.LevelRequest.RawName = map.Id;
            newCanvas.RefreshMapPreview();

            _entries[map.Id] = newCanvas;
        }

        /// <summary>
        /// When the map collection is modified through removal
        /// </summary>
        /// <param name="map"></param>
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