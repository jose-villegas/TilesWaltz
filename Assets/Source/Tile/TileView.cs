using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using TilesWalk.BaseInterfaces;
using TilesWalk.Building;
using TilesWalk.Extensions;
using TilesWalk.Gameplay;
using TilesWalk.General;
using TilesWalk.Tile.Rules;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace TilesWalk.Tile
{
	[ExecuteInEditMode]
	public class TileView : MonoBehaviour, IView
	{
		[SerializeField] private TileController _controller;

		private MeshRenderer _meshRenderer;
		private BoxCollider _collider;

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

		private static readonly Dictionary<TileColor, Material> Materials = new Dictionary<TileColor, Material>();

		private void Start()
		{
			if (Materials.Count == 0)
			{
				var colors = Enum.GetValues(typeof(TileColor));

				foreach (TileColor color in colors)
				{
					Materials[color] = new Material(Renderer.material) {color = color.Color()};
				}
			}

			// This small optimization enables us to share the material per color
			// instead of creating a new instance per every tile that tries to
			// change its color
			Renderer.material = Materials[_controller.Tile.TileColor];

			// update material on color update
			_controller.Tile
				.ObserveEveryValueChanged(x => x.TileColor)
				.Subscribe(UpdateColor)
				.AddTo(this);
		}

		private void UpdateColor(TileColor color)
		{
			Renderer.material = Materials[color];
		}

		#region Debug

#if UNITY_EDITOR
		[Header("Editor")] [SerializeField] private CardinalDirection direction = CardinalDirection.North;
		[SerializeField] private NeighborWalkRule rule = NeighborWalkRule.Plain;
		[Inject] private TileViewFactory _viewFactory;

		[Button]
		private void AddNeighbor()
		{
			if (!_controller.Tile.IsValidInsertion(direction, rule))
			{
				Debug.LogError("Cannot insert a neighbor here, space already occupied ");
				return;
			}

			var tile = _viewFactory.NewInstance();
			_controller.AddNeighbor(direction, rule, tile.Controller.Tile, transform, tile.transform);
			// add new insertion instruction for this tile
			_viewFactory.UpdateInstructions(this, tile, direction, rule);
		}

		[Button]
		private void Remove()
		{
			_controller.Remove();

			List<Tile> shufflePath = _controller.Tile.ShortestPathToLeaf;

			if (shufflePath == null || shufflePath.Count <= 0) return;

			// this structure with backup the origin position and rotations
			var backup = new List<Tuple<Vector3, Quaternion>>();
			var tiles = new List<TileView>();

			for (int i = 0; i < shufflePath.Count - 1; i++)
			{
				var source = _viewFactory.GetTileView(shufflePath[i]);
				var nextTo = _viewFactory.GetTileView(shufflePath[i + 1]);
				// backup info
				backup.Add(new Tuple<Vector3, Quaternion>(source.transform.position, source.transform.rotation));
				tiles.Add(source);
				// copy transform
				source.transform.position = nextTo.transform.position;
				source.transform.rotation = nextTo.transform.rotation;
			}

			var lastTile = _viewFactory.GetTileView(shufflePath[shufflePath.Count - 1]);
			var scale = lastTile.transform.localScale;
			lastTile.transform.localScale = Vector3.zero;

			StartCoroutine(ChainTowardsAnimation(tiles, backup))
				.GetAwaiter()
				.OnCompleted(() => { StartCoroutine(lastTile.LastShuffleTileAnimation(scale)); });
		}

		private IEnumerator LastShuffleTileAnimation(Vector3 scale)
		{
			while ((scale - transform.localScale).sqrMagnitude > Mathf.Epsilon)
			{
				var step = 6 * Time.deltaTime;
				transform.localScale = Vector3.MoveTowards(transform.localScale, scale, step);
				yield return new WaitForEndOfFrame();
			}
		}

		private IEnumerator ChainTowardsAnimation(List<TileView> tiles, List<Tuple<Vector3, Quaternion>> source)
		{
			for (int i = 0; i < tiles.Count && i < source.Count; i++)
			{
				var tile = tiles[i];

				var offset = source[i].Item1 - tile.transform.position;

				while ((source[i].Item1 - tile.transform.position).sqrMagnitude > Mathf.Epsilon ||
				       Quaternion.Angle(source[i].Item2, tile.transform.rotation) > Mathf.Epsilon)
				{
					var step = 20 * Time.deltaTime;
					tile.transform.position = Vector3.MoveTowards(tile.transform.position, source[i].Item1, step);
					tile.transform.rotation = Quaternion.RotateTowards(tile.transform.rotation, source[i].Item2, step * 50);
					yield return new WaitForEndOfFrame();
				}
			}
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

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;

			if (_controller.Tile.ShortestPathToLeaf != null)
			{
				foreach (var tile in _controller.Tile.ShortestPathToLeaf)
				{
					var view = _viewFactory.GetTileView(tile);
					Gizmos.DrawCube(view.transform.position + transform.up * 0.15f, Vector3.one * 0.25f);
				}
			}
		}
#endif

		#endregion
	}
}