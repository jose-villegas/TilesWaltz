﻿using System;
using System.Collections.Generic;
using System.Linq;
using TilesWalk.BaseInterfaces;
using TilesWalk.Building.Level;
using TilesWalk.Building.LevelEditor;
using TilesWalk.Extensions;
using TilesWalk.Gameplay;
using TilesWalk.Gameplay.Condition;
using TilesWalk.Gameplay.Display;
using TilesWalk.Gameplay.Score;
using TilesWalk.General;
using TilesWalk.General.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Tile.Level
{
    /// <summary>
    /// This class represents the tiles viewed during level game play
    /// </summary>
    public partial class LevelTileView : TileView, IView
    {
        [Inject] protected DiContainer _container;
        [Inject] protected TileViewLevelMap _tileLevelMap;
        [Inject] protected GameplaySetting _setting;
        [Inject] protected CanvasHoverListener _canvasHover;
        [Inject(Optional = true)] protected LevelFinishTracker _levelFinishTracker;

        private LevelTileViewTriggerBase _levelTileTriggerBase;
        private Animator _editorUIAnimator;
        private Transform _pathContainer;
        private Transform _wireBox;
        private GameColorPicker _pathColorPicker;

        protected Transform WireBox
        {
            get
            {
                if (_wireBox == null)
                {
                    _wireBox = transform.Find("WireBox");
                }

                return _wireBox;
            }
        }

        public Transform PathContainer
        {
            get
            {
                if (_pathContainer == null)
                {
                    _pathContainer = LevelTileUIAnimator.transform.Find("PathContainer");
                }

                return _pathContainer;
            }
        }

        protected GameColorPicker PathColorPicker
        {
            get
            {
                if (_pathColorPicker == null)
                {
                    _pathColorPicker = PathContainer.GetComponentInChildren<GameColorPicker>(true);
                }

                return _pathColorPicker;
            }
        }

        public Animator LevelTileUIAnimator
        {
            get
            {
                if (_editorUIAnimator == null)
                {
                    _editorUIAnimator = GetComponentInChildren<Animator>(true);
                    _editorUIAnimator.gameObject.SetActive(true);
                }

                return _editorUIAnimator;
            }
        }

        public LevelTileViewTriggerBase Trigger
        {
            get
            {
                if (_levelTileTriggerBase == null)
                {
                    _levelTileTriggerBase = gameObject.AddComponent<LevelTileViewTriggerBase>();
                }

                return _levelTileTriggerBase;
            }
            protected set => _levelTileTriggerBase = value;
        }

        public bool ShownGuide { get; set; }

        protected override void OnGamePaused(Unit u)
        {
        }
        protected override void OnGameResumed(Unit u)
        {
        }

        protected override void OnDestroy()
        {
            _tileLevelMap.State = TileLevelMapState.FreeMove;
            StopAllCoroutines();
        }

        /// <summary>
        /// Initializes callback listeners for level tiles
        /// </summary>
        protected override void Start()
        {
            base.Start();

            // update particles on power up
            _controller.Tile.OnTilePowerUpChangedAsObservable().Subscribe(UpdatePowerUpFX).AddTo(this);

            // on level finish stop interactions
            if (_levelFinishTracker != null)
            {
                _levelFinishTracker.OnLevelFinishAsObservable().Subscribe(OnLevelFinish).AddTo(this);
            }

            // check others tiles click
            _tileLevelMap.Trigger.OnTileClickedAsObservable().Subscribe(OnAnyTileClicked).AddTo(this);

            // disable map animations on level map
            var mapLayer = Animator.GetLayerIndex("Map");
            Animator.SetLayerWeight(mapLayer, 0f);

            if (_pathColorPicker == null) PathColorPicker.Name = GameColor.Blank;

            _setting.ShowGuides.Subscribe(OnGuideSettingsChanged).AddTo(this);

            // check if we need to activate a separator
            _tileLevelMap.OnLevelMapLoadedAsObservable().Subscribe(_ =>
            {
                CheckSeparator();
            }).AddTo(this);
        }

        private void OnGuideSettingsChanged(bool toggle)
        {
            if (toggle == false)
            {
                foreach (var levelTileView in _tileLevelMap.HashToTile)
                {
                    var tile = levelTileView.Value;

                    tile.LevelTileUIAnimator.SetBool("ShowPath", false);
                    tile.PathContainer.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// When two tiles are next to each other but are not connected. This method
        /// handles the activation of a visual separation with unrelated neighboring tiles
        /// </summary>
        public void CheckSeparator()
        {
            var index = Controller.Tile.Index;

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    for (int k = -1; k < 2; k++)
                    {
                        if (i == 0 && j == 0 && k == 0) continue;

                        var checkIndex = new Vector3Int(index.x + i, index.y + j, index.z + k);

                        if (_tileLevelMap.Indexes.TryGetValue(checkIndex, out var matching))
                        {
                            // check if any points up in the same direction
                            if (matching != null)
                            {
                                var matchUp = Math.Abs(Vector3.Dot(transform.up, matching.transform.up) - 1) <
                                              Constants.Tolerance;
                                var notRelated = !Controller.Tile.Neighbors.ContainsValue(matching.Controller.Tile);

                                if (!matchUp || !notRelated) continue;

                                var direction = matching.transform.position - transform.position;
                                direction.Normalize();

                                if (Math.Abs(Vector3.Dot(direction, transform.forward) - 1f) < Constants.Tolerance)
                                {
                                    ParticleSystems.ParticleFX["S-North"].Play();
                                }
                                else if (Math.Abs(Vector3.Dot(direction, -transform.forward) - 1f) <
                                         Constants.Tolerance)
                                {
                                    ParticleSystems.ParticleFX["S-South"].Play();
                                }
                                else if (Math.Abs(Vector3.Dot(direction, transform.right) - 1f) < Constants.Tolerance)
                                {
                                    ParticleSystems.ParticleFX["S-East"].Play();
                                }
                                else if (Math.Abs(Vector3.Dot(direction, -transform.right) - 1f) < Constants.Tolerance)
                                {
                                    ParticleSystems.ParticleFX["S-West"].Play();
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the special effects for different power ups
        /// </summary>
        /// <param name="power">A tuple with the source tile and the previous power up value</param>
        private void UpdatePowerUpFX(Tuple<Tile, TilePowerUp> power)
        {
            if (ParticleSystems.ParticleFX == null || ParticleSystems.ParticleFX.Count == 0) return;

            switch (power.Item1.PowerUp)
            {
                case TilePowerUp.None:
                    ParticleSystems["North"].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    ParticleSystems["South"].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    ParticleSystems["East"].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    ParticleSystems["West"].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    ParticleSystems["Color"].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    break;
                case TilePowerUp.NorthSouthLine:
                    ParticleSystems["North"].Play();
                    ParticleSystems["South"].Play();
                    break;
                case TilePowerUp.EastWestLine:
                    ParticleSystems["East"].Play();
                    ParticleSystems["West"].Play();
                    break;
                case TilePowerUp.ColorMatch:
                    ParticleSystems["Color"].Play();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(power), power, null);
            }
        }

        /// <summary>
        /// The update cycle checks for color matching combos
        /// </summary>
        private void Update()
        {
            // check for combos
            if (Controller.Tile.MatchingColorPatch != null && Controller.Tile.MatchingColorPatch.Count > 2)
            {
                RemoveCombo();
            }
        }



        /// <summary>
        /// Logic for when the level finish condition is reached
        /// </summary>
        /// <param name="score"></param>
        private void OnLevelFinish(LevelScore score)
        {
            _tileLevelMap.State = TileLevelMapState.Locked;
        }

        private void OnAnyTileClicked(Tile tile)
        {
            if (_setting.ShowGuides.Value)
            {
                var view = _tileLevelMap.GetTileView(tile);

                if (view != this)
                {
                    ShownGuide = false;
                }
            }
        }

        /// <summary>
        /// Tile click logic
        /// </summary>
        public virtual void OnMouseDown()
        {
            if (_canvasHover.IsUIOverride) return;

            Trigger.OnTileClicked?.OnNext(_controller.Tile);

            if (_setting.ShowGuides.Value)
            {
                if (!ShownGuide)
                {
                    _tileLevelMap.ShowGuide(Controller.Tile);
                    ShownGuide = true;
                }
                else
                {
                    if (_tileLevelMap.IsMovementLocked()) return;

                    if (_levelFinishTracker != null && _levelFinishTracker.IsFinished) return;

                    Remove();
                }
            }
            else
            {
                if (_tileLevelMap.IsMovementLocked()) return;

                if (_levelFinishTracker != null && _levelFinishTracker.IsFinished) return;

                Remove();
            }
        }

        /// <summary>
        /// This handles updates in the <see cref="Tile.TileColor"/> property
        /// </summary>
        /// <param name="color">A tuple with the updated tile and the previous color</param>
        protected override void UpdateColor(Tuple<Tile, TileColor> color)
        {
            Renderer.material = _colorHandler.GetMaterial(color.Item1.TileColor);

            if (_controller.Tile.PowerUp == TilePowerUp.ColorMatch)
            {
                ParticleSystems["Color"].Stop();
                var pColor = _colorsConfiguration[_controller.Tile.TileColor];
                ParticleSystem.MainModule settings = ParticleSystems["Color"].main;
                settings.startColor = pColor;
                ParticleSystems["Color"].Play();
            }
        }

        #region Debug

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(Controller.Tile.Index, Vector3.one);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;

            if (_controller.Tile.ShortestPathToLeaf != null)
            {
                foreach (var tile in _controller.Tile.ShortestPathToLeaf)
                {
                    if (!_tileLevelMap.HasTileView(tile)) continue;

                    var view = _tileLevelMap.GetTileView(tile);
                    Gizmos.DrawCube(view.transform.position +
                                    view.transform.up * 0.15f, Vector3.one * 0.15f);
                }
            }

            Gizmos.color = Color.blue;
            foreach (var hingePoint in _controller.Tile.HingePoints)
            {
                var relative = transform.rotation * hingePoint.Value;
                Gizmos.DrawSphere(transform.position + relative, 0.05f);

                if (!_tileLevelMap.HasTileView(_controller.Tile.Neighbors[hingePoint.Key])) continue;

                var view = _tileLevelMap.GetTileView(_controller.Tile.Neighbors[hingePoint.Key]);
                var joint = view.transform.rotation * view.Controller.Tile.HingePoints[hingePoint.Key.Opposite()];
                Gizmos.DrawLine(transform.position + relative, view.transform.position + joint);
            }

            Gizmos.color = Color.magenta;
            if (_controller.Tile.MatchingColorPatch != null && _controller.Tile.MatchingColorPatch.Count > 2)
            {
                foreach (var tile in _controller.Tile.MatchingColorPatch)
                {
                    if (!_tileLevelMap.HasTileView(tile)) continue;

                    var view = _tileLevelMap.GetTileView(tile);
                    Gizmos.DrawWireCube(view.transform.position, Vector3.one);
                }
            }
        }
#endif

        #endregion
    }
}