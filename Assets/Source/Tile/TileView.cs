using NaughtyAttributes;
using System;
using System.ComponentModel;
using System.Linq;
using TilesWalk.BaseInterfaces;
using TilesWalk.Building;
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

		private MeshRenderer _meshRenderer;
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

		private MeshRenderer Renderer
		{
			get
			{
				if (_meshRenderer == null)
				{
					_meshRenderer = GetComponent<MeshRenderer>();
				}

				return _meshRenderer;
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

		private void OnDestroy()
		{
			foreach (var item in _controller.Tile.Neighbors)
			{
				item.Value.Neighbors.Remove(item.Key.Opposite());
				item.Value.HingePoints.Remove(item.Key.Opposite());
			}
		}

		private void Start()
		{
			Renderer.material.color = _controller.Tile.Color;
		}

		#region Debug

#if UNITY_EDITOR
		[Header("Editor")] [SerializeField] private CardinalDirection direction = CardinalDirection.North;
		[SerializeField] private NeighborWalkRule rule = NeighborWalkRule.Plain;
		[Inject] private TileGenerator _generator;

		[Button]
		private void AddNeighbor()
		{
			if (!_controller.Tile.IsValidInsertion(direction, rule))
			{
				Debug.LogError("Cannot insert a neighbor here, space already occupied ");
				return;
			}

			var tile = _generator.Generate();
			_controller.AddNeighbor(direction, rule, tile.Controller.Tile, transform, tile.transform);
			// add new insertion instruction for this tile
			_generator.UpdateInstructions(this, tile, direction, rule);
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