using NaughtyAttributes;
using System;
using System.ComponentModel;
using System.Linq;
using TilesWalk.BaseInterfaces;
using TilesWalk.Extensions;
using TilesWalk.General;
using TilesWalk.Tile.Rules;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace TilesWalk.Tile
{
	public class TileView : MonoBehaviour, IView
	{
		[SerializeField]
		private TileController _controller;

		public TileController Controller { get => _controller; }

		public TileView()
		{
			_controller = new TileController();
		}

		private void OnEnable()
		{
			_controller.Tile.PropertyChanged += PropertyChanged;
		}

		private void OnDisable()
		{
			_controller.Tile.PropertyChanged -= PropertyChanged;
		}

		public void PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var tile = _controller.Tile;

			transform.position = Vector3.zero;
			transform.rotation = Quaternion.identity;

			switch (tile.Rule)
			{
				case NeighborWalkRule.Up:
					transform.RotateAround(Vector3.zero, Vector3.left, 90);
					break;
				case NeighborWalkRule.Plain:
					transform.rotation = Quaternion.identity;
					break;
				case NeighborWalkRule.Down:
					transform.RotateAround(Vector3.zero, Vector3.left, 90);
					break;
			}

			transform.position = tile.Position;
		}

		private void OnDrawGizmos()
		{
			var bounds = _controller.Tile.Bounds;

			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(bounds.center, _controller.Tile.Bounds.size);

			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(_controller.Tile.Index, Vector3.one);

			var tilePoints = _controller.Tile.HingePoints(CardinalDirection.North);
			tilePoints = tilePoints.Concat(_controller.Tile.HingePoints(CardinalDirection.South)).ToArray();

			Gizmos.color = Color.blue;
			for (int i = 0; i < tilePoints.Length; i++)
			{
				Gizmos.DrawSphere(tilePoints[i], 0.05f);
			}
		}

		private void OnDestroy()
		{
			foreach (var item in _controller.Tile.Neighbors)
			{
				item.Value.Neighbors.Remove(item.Key.Opposite());
			}
		}

#if UNITY_EDITOR
		[Header("Editor")]
		[SerializeField]
		private CardinalDirection direction = CardinalDirection.North;
		[SerializeField]
		private NeighborWalkRule rule = NeighborWalkRule.Plain;
		[Inject(Id = "TileAsset")]
		private AssetReference _tileAsset;


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

				// Obtain proper boundaries from collider
				var boxCollider = handle.Result.GetComponent<BoxCollider>();
				view.Controller.AdjustBounds(boxCollider.bounds);

				_controller.AddNeighbor(direction, rule, view.Controller.Tile);
			};
		}
#endif
	}
}

