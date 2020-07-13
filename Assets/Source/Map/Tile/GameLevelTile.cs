using System;
using System.Collections.Generic;
using System.Linq;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Input;
using TilesWalk.Gameplay.Score;
using TilesWalk.General;
using TilesWalk.General.FX;
using TilesWalk.General.UI;
using TilesWalk.Map.Bridge;
using TilesWalk.Map.Scaffolding;
using TilesWalk.Navigation.UI;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace TilesWalk.Map.Tile
{
    /// <summary>
    /// The game level tiles are tiles in the game map used to represent
    /// entries to game levels
    /// </summary>
    public class GameLevelTile : ObservableTriggerBase, ILevelNameRequire
    {
        [Inject] private LevelMapDetailsCanvas _detailsCanvas;
        [Inject] private GameScoresHelper _gameScoresHelper;
        [Inject] private LevelStateTileMaterialHandler _colorHandler;
        [Inject] private LevelStarsTileMaterialHandler _starsColorHandler;
        [Inject] private GameEventsHandler _gameEvents;
        [Inject] private CanvasHoverListener _canvasHover;

        private GameLevelTileLinksHandler _links;

        public ReactiveProperty<string> Name { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<LevelMap> Map { get; } = new ReactiveProperty<LevelMap>();

        /// <summary>
        /// This class holds the links that relate levels through
        /// the map structure
        /// </summary>
        public GameLevelTileLinksHandler Links
        {
            get
            {
                if (_links == null)
                {
                    _links = GetComponent<GameLevelTileLinksHandler>();
                }

                return _links;
            }
        }

        private Subject<GameLevelTile> _onLevelTileClick;
        private Subject<GameLevelTile> _onLevelDataLoaded;
        private bool _disabledClick;
        private GameMapTile _mapTile;

        /// <summary>
        /// The tile mesh renderer
        /// </summary>
        public GameMapTile MapTile
        {
            get
            {
                if (_mapTile == null)
                {
                    _mapTile = GetComponentInParent<GameMapTile>();
                }

                return _mapTile;
            }
        }

        private void OnMouseDown()
        {
            if (_disabledClick) return;

            if (_canvasHover.IsUIOverride) return;

            OnMapTileClick();
        }

        /// <summary>
        /// Logic for when a level tile is clicked, this shows
        /// the level details in the UI
        /// </summary>
        public void OnMapTileClick()
        {
            if (_detailsCanvas.IsVisible && _detailsCanvas.LevelRequest.Name.Value == Name.Value)
            {
                _detailsCanvas.Hide();
            }
            else
            {
                _detailsCanvas.LevelRequest.Name.Value = Name.Value;
                _detailsCanvas.Show();
            }

            _onLevelTileClick?.OnNext(this);
        }

        /// <summary>
        /// Raised when a tile is clicked
        /// </summary>
        /// <returns></returns>
        public IObservable<GameLevelTile> OnLevelTileClickAsObservable()
        {
            return _onLevelTileClick = _onLevelTileClick ?? new Subject<GameLevelTile>();
        }

        /// <summary>
        /// Raised when the level tile is fully loaded
        /// </summary>
        /// <returns></returns>
        public IObservable<GameLevelTile> OnLevelDataLoadedAsObservable()
        {
            return _onLevelDataLoaded = _onLevelDataLoaded ?? new Subject<GameLevelTile>();
        }

        protected override void RaiseOnCompletedOnDestroy()
        {
            _onLevelTileClick?.OnCompleted();
            _onLevelDataLoaded?.OnCompleted();
        }

        private void Start()
        {
            // Subscribe to game events
            _gameEvents.OnGameResumedAsObservable().Subscribe(OnGameResumed).AddTo(this);
            _gameEvents.OnGamePausedAsObservable().Subscribe(OnGamePaused).AddTo(this);

            // initialize particle effects
            _detailsCanvas.LevelRequest.Name.Subscribe(mapName =>
            {
                if (mapName == Name.Value)
                {
                    if (!_gameScoresHelper.IsCompleted(Map.Value))
                    {
                        MapTile.ParticleSystems["Completed"].Stop();

                        MapTile.ParticleSystems["ToComplete"].gameObject.SetActive(true);
                        MapTile.ParticleSystems["ToComplete"].Play();
                    }
                    else
                    {
                        MapTile.ParticleSystems["ToComplete"].Stop();

                        MapTile.ParticleSystems["Completed"].gameObject.SetActive(true);
                        MapTile.ParticleSystems["Completed"].Play();
                    }
                }
                else
                {
                    MapTile.ParticleSystems.StopAll();
                }
            }).AddTo(this);

            // Initialize materials
            Map.Subscribe(map =>
            {
                if (map != null)
                {
                    if (_gameScoresHelper.IsCompleted(map))
                    {
                        MapTile.Renderer.material = _starsColorHandler.GetMaterial(Map.Value.StarsRequired);
                    }
                    else
                    {
                        if (_gameScoresHelper.GameStars < map.StarsRequired)
                        {
                            MapTile.Renderer.material = _colorHandler.GetMaterial(LevelMapState.Locked);
                        }
                        else
                        {
                            MapTile.Renderer.material = _colorHandler.GetMaterial(LevelMapState.ToComplete);
                        }
                    }

                    _onLevelDataLoaded?.OnNext(this);
                }
            });
        }

        private void OnGamePaused(Unit obj)
        {
            _disabledClick = true;
        }

        private void OnGameResumed(Unit obj)
        {
            _disabledClick = false;
        }
    }
}