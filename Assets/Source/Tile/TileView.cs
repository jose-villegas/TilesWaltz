using System;
using TilesWalk.BaseInterfaces;
using TilesWalk.Gameplay.Display;
using TilesWalk.Gameplay.Input;
using TilesWalk.General.FX;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Tile
{
	public abstract class TileView : MonoBehaviour, IView
	{
		[Inject] protected GameTileColorsConfiguration _colorsConfiguration;
		[Inject] protected TileColorMaterialColorMatchHandler _colorHandler;
		[Inject] protected GameEventsHandler _gameEvents;

		[SerializeField] protected TileController _controller = new TileController();

		private MeshRenderer _meshRenderer;
		private BoxCollider _collider;
		private ParticleSystemsCollector _particleSystems;
        private Animator _animator;

        public TileController Controller
		{
			get => _controller;
			set => _controller = value;
		}

		public BoxCollider Collider
		{
			get
			{
				if (_collider == null)
				{
					_collider = GetComponentInChildren<BoxCollider>();
				}

				return _collider;
			}
		}

        public MeshRenderer Renderer
		{
			get
			{
				if (_meshRenderer == null)
				{
					_meshRenderer = GetComponentInChildren<MeshRenderer>();
				}

				return _meshRenderer;
			}
		}

		public ParticleSystemsCollector ParticleSystems
		{
			get
			{
				if (_particleSystems == null)
				{
					_particleSystems = gameObject.AddComponent<ParticleSystemsCollector>();
				}

				return _particleSystems;
			}
			protected set => _particleSystems = value;
		}

        public Animator Animator
        {
            get
            {
                if (_animator == null)
                {
                    _animator = GetComponentInParent<Animator>();
                }

                return _animator;
            }
        }

		protected abstract void UpdateColor(Tuple<Tile, TileColor> color);

		protected virtual void Start()
		{
			// This small optimization enables us to share the material per color
			// instead of creating a new instance per every tile that tries to
			// change its color
			Renderer.material = _colorHandler.GetMaterial(_controller.Tile.TileColor);
			// update material on color update
			_controller.Tile.OnTileColorChangedAsObservable().Subscribe(UpdateColor).AddTo(this);

			_gameEvents.OnGamePausedAsObservable().Subscribe(OnGamePaused);
			_gameEvents.OnGameResumedAsObservable().Subscribe(OnGameResumed);
		}

		protected abstract void OnGameResumed(Unit u);
		protected abstract void OnGamePaused(Unit u);
		
		protected virtual void OnDestroy()
		{
		}
	}
}