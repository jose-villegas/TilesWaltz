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
    /// <summary>
    /// Abstract tile view class, generalization for all sort of tile objects in the game
    /// </summary>
    public abstract class TileView : MonoBehaviour, IView
    {
        /// <summary>
        /// Color configuration for tiles
        /// </summary>
        [Inject] protected GameTileColorsConfiguration _colorsConfiguration;

        /// <summary>
        /// The tile color material handler
        /// </summary>
        [Inject] protected TileColorMaterialColorMatchHandler _colorHandler;

        /// <summary>
        /// The game events handler
        /// </summary>
        [Inject] protected GameEventsHandler _gameEvents;

        /// <summary>
        /// The controller
        /// </summary>
        [SerializeField] protected TileController _controller = new TileController();

        private MeshRenderer _meshRenderer;
        private BoxCollider _collider;
        private ParticleSystemsCollector _particleSystems;
        private Animator _animator;

        /// <summary>
        /// The tile view controller
        /// </summary>
        public TileController Controller
        {
            get => _controller;
            set => _controller = value;
        }

        /// <summary>
        /// This obtains the model box collider
        /// </summary>
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

        /// <summary>
        /// This obtains the model mesh renderer
        /// </summary>
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

        /// <summary>
        /// This obtains a collector of particle systems for the tile
        /// </summary>
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

        /// <summary>
        /// The parent animator, handles all sort of animations over the whole tile structure
        /// </summary>
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

        /// <summary>
        /// This method is called when the <see cref="Tile.TileColor"/> property
        /// value is changed
        /// </summary>
        /// <param name="color">A tuple with the updated tile and its previous color</param>
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

        /// <summary>
        /// Called on game resume
        /// </summary>
        /// <param name="u"></param>
        protected abstract void OnGameResumed(Unit u);

        /// <summary>
        /// Called on game pause
        /// </summary>
        /// <param name="u"></param>
        protected abstract void OnGamePaused(Unit u);

        /// <summary>
        /// Called when the object is destroyed
        /// </summary>
        protected abstract void OnDestroy();
    }
}