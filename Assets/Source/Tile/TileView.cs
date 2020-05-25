using NaughtyAttributes;
using System;
using System.ComponentModel;
using System.Linq;
using TilesWalk.BaseInterfaces;
using TilesWalk.Extensions;
using TilesWalk.General;
using TilesWalk.Tile.Rules;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace TilesWalk.Tile
{
	[ExecuteInEditMode]
	public class TileView : MonoBehaviour, IView
	{
		[SerializeField] private TileController _controller;

		private BoxCollider _collider;
		private IDisposable _positionDisposable;
		private IDisposable _rotationDisposable;
		private IDisposable _scaleDisposable;

		private BoxCollider Collider
		{
			get
			{
				if (_collider == null)
				{
					_collider = GetComponent<BoxCollider>();
				}

				return _collider;
			}
		}

		public TileController Controller
		{
			get => _controller;
		}

		public TileView()
		{
			_controller = new TileController();
		}

		private void OnEnable()
		{
			_controller.Tile.Transform.SubscribeTransform(transform);
		}

		private void OnDisable()
		{
			_controller.Tile.Transform.Unsubscribe();
		}

		private void OnDestroy()
		{
			foreach (var item in _controller.Tile.Neighbors)
			{
				item.Value.Neighbors.Remove(item.Key.Opposite());
				item.Value.HingePoints.Remove(item.Key.Opposite());
			}
		}

		#region Debug

#if UNITY_EDITOR
		[Header("Editor")] [SerializeField] private CardinalDirection direction = CardinalDirection.North;
		[SerializeField] private NeighborWalkRule rule = NeighborWalkRule.Plain;
		[Inject(Id = "TileAsset")] private AssetReference _tileAsset;

		[Button]
		private void AddNeighbor()
		{
			if (!_controller.Tile.IsValidInsertion(direction, rule))
			{
				Debug.LogError("Cannot insert a neighbor here, space already occupied ");
				return;
			}

			_tileAsset.InstantiateAsync(Vector3.zero, Quaternion.identity, transform.parent).Completed += (handle) =>
			{
				var view = handle.Result.AddComponent<TileView>();
				view._tileAsset = _tileAsset;
				view.rule = rule;
				view.direction = direction;

				// Obtain proper boundaries from collider
				var boxCollider = handle.Result.GetComponent<BoxCollider>();
				view.Controller.AdjustBounds(boxCollider.bounds);

				_controller.AddNeighbor(direction, rule, view.Controller.Tile);
			};
		}

		private void OnDrawGizmos()
		{
			var tile = _controller.Tile;

			Gizmos.color = Color.blue;
			foreach (var value in tile.HingePoints.Values)
			{
				var translate = transform.position - tile.Index;
				Gizmos.DrawSphere(translate + value, 0.05f);
			}
		}
#endif

		#endregion
	}
}