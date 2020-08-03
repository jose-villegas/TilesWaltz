using System;
using System.Collections.Generic;
using TilesWalk.Building.Level;
using TilesWalk.Building.LevelEditor.UI;
using TilesWalk.Gameplay.Condition;
using TilesWalk.Gameplay.Score;
using TilesWalk.General;
using TilesWalk.General.UI;
using TilesWalk.Map.Bridge;
using TilesWalk.Map.General;
using TilesWalk.Map.Scaffolding;
using TilesWalk.Map.Tile;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TilesWalk.Navigation.UI
{
    /// <summary>
    /// This component handles the panel for showing level details in the game map scene
    /// </summary>
    public class LevelMapDetailsCanvas : CanvasGroupBehaviour
    {
        [Inject] private LevelBridge _levelBridge;
        [Inject] private GameLevelTilesInitializer _gameLevelTilesInitializer;
        [Inject] private GameScoresHelper _gameScoresHelper;
        [Inject] private MapProviderSolver _solver;

        [SerializeField] private LevelNameRequestHandler _levelRequest;
        [SerializeField] private Button _playButton;
        [SerializeField] private CanvasGroupBehaviour _timeConditionContainer;
        [SerializeField] private CanvasGroupBehaviour _moveConditionContainer;
        [SerializeField] private List<RectTransform> _panelArea;

        [Header("Navigation")] [SerializeField]
        private List<DirectionButton> _directionButtons;

        private Camera _canvasCamera;
        private IDisposable _clicksListener;

        public LevelNameRequestHandler LevelRequest => _levelRequest;

        private void Awake()
        {
            _solver.InstanceProvider(gameObject);
            _levelRequest.OnTileMapFoundAsObservable().Subscribe(UpdateCanvas).AddTo(this);

            for (int i = 0; i < _directionButtons.Count; i++)
            {
                var directionButton = _directionButtons[i];

                directionButton.Button.onClick.AddListener(() =>
                {
                    switch (directionButton.Direction)
                    {
                        case CardinalDirection.North:
                            EnterDirection = Vector2.down * 3;
                            ExitDirection = -EnterDirection;
                            break;
                        case CardinalDirection.South:
                            EnterDirection = Vector2.up * 3;
                            ExitDirection = -EnterDirection;
                            break;
                        case CardinalDirection.East:
                            EnterDirection = Vector2.left * 3;
                            ExitDirection = -EnterDirection;
                            break;
                        case CardinalDirection.West:
                            EnterDirection = Vector2.right * 3;
                            ExitDirection = -EnterDirection;
                            break;
                    }


                    var levelTile = _gameLevelTilesInitializer[_levelRequest.Map];
                    var neighbor = levelTile.Links[directionButton.Direction];

                    OnHideAsObservable().Take(1).Subscribe(u => { neighbor.OnMapTileClick(); }).AddTo(this);

                    Hide();
                });
            }

            var canvas = GetComponentInParent<Canvas>();
            _canvasCamera = canvas.worldCamera;
        }

        public override void Show()
        {
            _clicksListener?.Dispose();
            OnShowAsObservable().Take(1).Subscribe(_ =>
            {
                _clicksListener = Observable.EveryUpdate().Where(__ => Input.GetMouseButton(0))
                    .Subscribe(OnMouseClick);
            }).AddTo(this);

            base.Show();
        }

        public override void Hide()
        {
            _clicksListener?.Dispose();
            base.Hide();
        }

        /// <summary>
        /// Determines if the user clicks outside the canvas, if that's the case, it closes the <see cref="CanvasGroupBehaviour"/>
        /// </summary>
        /// <param name="tick"></param>
        private void OnMouseClick(long tick)
        {
            var clickOutside = true;

            for (int i = 0; i < _panelArea.Count; i++)
            {
                var area = _panelArea[i];

                clickOutside &=
                    !RectTransformUtility.RectangleContainsScreenPoint(area, Input.mousePosition, _canvasCamera);
            }

            if (IsVisible && clickOutside)
            {
                Hide();
            }
        }

        /// <summary>
        /// Update the panel content with the given <see cref="LevelMap"/>
        /// </summary>
        /// <param name="map">The map to show details of</param>
        private void UpdateCanvas(LevelMap map)
        {
            //_playButton.interactable = _gameScoresHelper.GameStars >= map.StarsRequired;

            // setup the bridge before loading scene
            _playButton.onClick.AddListener(() =>
            {
                // prepare the bridge
                _levelBridge.Payload = new LevelBridgePayload(_levelRequest.Map, _levelRequest.Condition,
                    _gameScoresHelper.State(_levelRequest.Map));
            });

            //// set condition
            if (map.FinishCondition == FinishCondition.MovesLimit)
            {
                _moveConditionContainer.Show();
                _timeConditionContainer.Hide();
            }
            else if (map.FinishCondition == FinishCondition.TimeLimit)
            {
                _moveConditionContainer.Hide();
                _timeConditionContainer.Show();
            }

            // set navigation buttons
            var levelTile = _gameLevelTilesInitializer[map];

            for (int i = 0; i < _directionButtons.Count; i++)
            {
                var directionButton = _directionButtons[i];
                directionButton.Button.image.enabled = levelTile.Links.HasNeighbor(directionButton.Direction);
                directionButton.Button.interactable = levelTile.Links.HasNeighbor(directionButton.Direction);
            }
        }
    }
}