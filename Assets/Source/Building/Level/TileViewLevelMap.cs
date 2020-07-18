using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Newtonsoft.Json;
using TilesWalk.Building.LevelEditor;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Animation;
using TilesWalk.Gameplay.Condition;
using TilesWalk.General;
using TilesWalk.Map.Bridge;
using TilesWalk.Tile.Level;
using UniRx;
using UnityEngine;
using Zenject;
using LevelTileView = TilesWalk.Tile.Level.LevelTileView;

namespace TilesWalk.Building.Level
{
    /// <summary>
    /// This class holds the currently running level map and all its <see cref="LevelTileView"/>
    /// instances
    /// </summary>
    public class TileViewLevelMap : TileViewMap<LevelMap, LevelTileView, LevelTileViewFactory>
    {
        [Inject] private LevelBridge _levelBridge;
        [Inject] protected AnimationConfiguration _animationSettings;

        [SerializeField] private LevelLoadOptions _loadOption;

        [SerializeField]
        private Dictionary<Vector3Int, LevelTileView> _indexes = new Dictionary<Vector3Int, LevelTileView>();

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
        }

        /// <summary>
        /// This property handles the current map state, useful for locking movement
        /// or seeing what is actually happening with the tiles
        /// </summary>
        public TileLevelMapState State
        {
            get => _state;
            set
            {
                _state = value;
                _onMapStateChanged?.OnNext(_state);
            }
        }


        public LevelLoadOptions LoadOption => _loadOption;

        public Dictionary<Vector3Int, LevelTileView> Indexes => _indexes;

        public List<Tile.Tile> CurrentPathShown { get; set; }

        private TileLevelMapState _state;
        private LevelTileViewTriggerBase _levelTileTriggerBase;
        private TileLevelMapState _backupState;


        /// <summary>
        /// Checks if any of the current states for the map is locking movement
        /// </summary>
        /// <returns></returns>
        public bool IsMovementLocked()
        {
            return State == TileLevelMapState.RemovingTile ||
                   State == TileLevelMapState.OnComboRemoval ||
                   State == TileLevelMapState.OnPowerUpRemoval ||
                   State == TileLevelMapState.EditorMode ||
                   State == TileLevelMapState.Locked;
        }

        protected override void Start()
        {
            base.Start();

            switch (_loadOption)
            {
                case LevelLoadOptions.FromInstructions:
                    BuildFromInstructions();
                    break;
                case LevelLoadOptions.FromBridgeLevelMode:
                    BuildTileMap<LevelTileView>(_levelBridge.Payload.Level);
                    break;
                case LevelLoadOptions.FromBridgeEditorMode when _levelBridge.Payload == null:
                    BuildEditorRootMap();
                    break;
                case LevelLoadOptions.FromBridgeEditorMode:
                    BuildTileMap<LevelEditorTileView>(_levelBridge.Payload.Level);
                    break;
                case LevelLoadOptions.LevelEditor:
                    BuildEditorRootMap();
                    break;
            }
        }

        /// <summary>
        /// This creates a single tile map, with a 'root' tile
        /// </summary>
        private void BuildEditorRootMap()
        {
            var customLevel = new LevelMap
            {
                Id = Constants.CustomLevelName,
                Instructions = new List<InsertionInstruction>(),
                Roots = new List<RootTile>()
                {
                    new RootTile()
                    {
                        Key = 0,
                        Position = Vector3.zero,
                        Rotation = Vector3.zero
                    }
                },
                MapSize = 3,
                StarsRequired = 0,
                Target = 0,
                FinishCondition = FinishCondition.MovesLimit,
            };
            // add the root tile
            BuildTileMap<LevelEditorTileView>(customLevel);
        }

        /// <summary>
        /// Get tile callbacks and generalize them for the whole map,
        /// the callbacks are subscribed to the tile, so when they are disposed
        /// with the tile instance
        /// </summary>
        /// <param name="tile"></param>
        protected override void OnNewTileInstance(LevelTileView tile)
        {
            tile.Trigger.OnTileRemovedAsObservable()
                .Subscribe(path => Trigger.OnTileRemoved?.OnNext(path)).AddTo(tile);
            tile.Trigger.OnTileClickedAsObservable()
                .Subscribe(val => Trigger.OnTileClicked?.OnNext(val)).AddTo(tile);
            // level related events
            tile.Trigger.OnComboRemovalAsObservable()
                .Subscribe(path => Trigger.OnComboRemoval?.OnNext(path)).AddTo(tile);
            tile.Trigger.OnPowerUpRemovalAsObservable()
                .Subscribe(path => Trigger.OnPowerUpRemoval?.OnNext(path)).AddTo(tile);
        }

        public override void RegisterTile(LevelTileView tile, int? hash = null)
        {
            if (TileToHash.ContainsKey(tile))
            {
                var h = TileToHash[tile];
                TileToHash.Remove(tile);
                HashToTile.Remove(h);
            }

            var id = hash ?? tile.GetHashCode();
            TileToHash[tile] = id;
            HashToTile[id] = tile;
            TileView[tile.Controller.Tile] = tile;
            // trigger event
            _onTileRegistered?.OnNext(tile);
        }

        /// <summary>
        /// This method check if any color matching combo existing within all the tiles
        /// This is useful for preventing level finish from getting the last combo points
        /// </summary>
        /// <returns></returns>
        public bool IsAnyComboLeft()
        {
            foreach (var value in TileView.Values)
            {
                if (value.Controller.Tile.MatchingColorPatch.Count > 2)
                {
                    return true;
                }
            }

            return false;
        }

        public override void RemoveTile(LevelTileView tile)
        {
            if (!TileToHash.TryGetValue(tile, out var hash)) return;

            TileToHash.Remove(tile);
            HashToTile.Remove(hash);
            TileView.Remove(tile.Controller.Tile);
            // remove from map
            _map.Instructions.RemoveAll(x => x.Tile == hash);
            _map.Instructions.RemoveAll(x => x.Root == hash);

            if (!tile.Controller.Tile.IsLeaf())
            {
                foreach (var tileNeighbor in tile.Controller.Tile.Neighbors)
                {
                    if (tileNeighbor.Value.Root)
                    {
                        continue;
                    }

                    if (!tileNeighbor.Value.IsConnectedToRoot())
                    {
                        tileNeighbor.Value.Root = true;
                        var view = TileView[tileNeighbor.Value];

                        _map.Roots.Add(new RootTile()
                        {
                            Key = TileToHash[view],
                            Position = view.transform.position,
                            Rotation = view.transform.eulerAngles
                        });
                    }
                }
            }

            // update indexes
            if (_indexes.TryGetValue(tile.Controller.Tile.Index, out var matchingIndexes))
            {
                _indexes.Remove(tile.Controller.Tile.Index);
            }

            // remove all instructions that refer to this tile
            Insertions.Remove(hash);

            foreach (var insertion in Insertions.ToList())
            {
                // remove these related entries
                insertion.Value.RemoveAll(x => x.Root == hash);
                insertion.Value.RemoveAll(x => x.Tile == hash);
            }
        }


        [Button]
        public void RefreshAllPaths()
        {
            foreach (var viewKey in TileView.Keys)
            {
                viewKey.RefreshShortestLeafPath();
                viewKey.RefreshMatchingColorPatch();
            }
        }

        [Button]
        public void GenerateInstructions()
        {
            var instr = Insertions.Values.SelectMany(x => x).ToList();

            var map = new LevelMap()
            {
                Instructions = instr,
                Roots = HashToTile
                    .Where(x => x.Value.Controller.Tile.Root)
                    .Select(val => new RootTile()
                    {
                        Key = val.Key,
                        Position = val.Value.transform.position,
                        Rotation = val.Value.transform.eulerAngles
                    }).ToList()
            };

            _instructions = JsonConvert.SerializeObject(map);
        }

        [Button]
        public void BuildFromInstructions()
        {
            // first instance all the needed tiles
            var map = JsonConvert.DeserializeObject<LevelMap>(_instructions);

            switch (_loadOption)
            {
                case LevelLoadOptions.None:
                case LevelLoadOptions.FromInstructions:
                case LevelLoadOptions.FromBridgeLevelMode:
                    BuildTileMap<LevelTileView>(map);
                    break;
                case LevelLoadOptions.LevelEditor:
                case LevelLoadOptions.FromBridgeEditorMode:
                    BuildTileMap<LevelEditorTileView>(map);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void BuildTileMap<T>(LevelMap map)
        {
            Reset();
            List<int> currentRoots = new List<int>();

            // create root tiles first
            foreach (var rootTile in map.Roots)
            {
                T tile = null;
                tile = _factory.NewInstance<T>();
                RegisterTile(tile, rootTile.Key);
                currentRoots.Add(rootTile.Key);
                // set transform
                tile.transform.position = rootTile.Position;
                tile.transform.rotation = Quaternion.Euler(rootTile.Rotation);
                // register root within the tile map
                tile.Controller.Tile.Root = true;
                _map.Roots.Add(rootTile);
                // update indexes
                UpdateIndexes(tile);
            }

            var hasNewRootAvailable = true;

            // Now execute neighbor insertion logic
            while (hasNewRootAvailable)
            {
                var newRoots = new List<int>();
                hasNewRootAvailable = false;

                // now create all the tiles related to these roots
                foreach (var root in currentRoots)
                {
                    var related = map.Instructions.Where(x => x.Root == root).ToList();
                    var anyRelated = related.Any();
                    // ensure next loop since there is at least another tile parent
                    hasNewRootAvailable |= anyRelated;

                    // no need to iterate if this tile is a leaf
                    if (!anyRelated) continue;

                    foreach (var instruction in related)
                    {
                        var rootTile = HashToTile[instruction.Root];
                        // create neighbor instance
                        T tile = null;
                        tile = _factory.NewInstance<T>();
                        RegisterTile(tile, instruction.Tile);
                        // insert on the tile structure
                        var insert = HashToTile[instruction.Tile];
                        rootTile.InsertNeighbor(instruction.Direction, instruction.Rule, insert);
                        // update indexes
                        UpdateIndexes(tile);
                        // register to structure
                        UpdateInstructions(rootTile, insert, instruction.Direction, instruction.Rule);
                        // update newer roots for next loop
                        newRoots.Add(instruction.Tile);
                    }
                }

                currentRoots = newRoots;
            }

            _map.Id = map.Id;
            _map.Target = map.Target;
            _map.FinishCondition = map.FinishCondition;
            _map.MapSize = map.MapSize;
            _onLevelMapLoaded?.OnNext(_map);
        }

        public void HideGuide()
        {
            // first hide the previous path
            if (CurrentPathShown != null && CurrentPathShown.Count >= 0)
            {
                for (var index = 0; index < CurrentPathShown.Count; index++)
                {
                    var tile = CurrentPathShown[index];

                    if (!HasTileView(tile)) continue;

                    var view = GetTileView(tile);

                    view.LevelTileUIAnimator.SetBool("ShowPath", false);
                    view.PathContainer.gameObject.SetActive(false);
                }
            }

            CurrentPathShown = null;
        }

        public void ShowGuide(Tile.Tile tile)
        {
            HideGuide();

            // now handle the newest path
            var tileShortestPathToLeaf = tile.ShortestPathToLeaf;

            if (tileShortestPathToLeaf == null || tileShortestPathToLeaf.Count == 0) return;

            CurrentPathShown = tileShortestPathToLeaf;

            var animDuration = _animationSettings.PathGuideAnimationTime;
            var steps = animDuration / tileShortestPathToLeaf.Count;

            for (var index = 0; index < tileShortestPathToLeaf.Count; index++)
            {
                var current = tileShortestPathToLeaf[index];

                if (!HasTileView(current)) continue;

                var view = GetTileView(current);

                Observable.Timer(TimeSpan.FromSeconds(index * steps)).Subscribe(_ => { }, () =>
                {
                    view.PathContainer.gameObject.SetActive(true);
                    view.LevelTileUIAnimator.SetBool("ShowPath", true);
                }).AddTo(this);
            }
        }

        public void UpdateIndexes<T>(T tile) where T : LevelTileView
        {
            if (!_indexes.TryGetValue(tile.Controller.Tile.Index, out var matching))
            {
                _indexes[tile.Controller.Tile.Index] = tile;
            }
        }

        /// <summary>
        /// Handles game resume
        /// </summary>
        /// <param name="u"></param>
        protected void OnGameResumed(Unit u)
        {
            State = _backupState;
        }

        /// <summary>
        /// Handles game pause
        /// </summary>
        /// <param name="u"></param>
        protected void OnGamePaused(Unit u)
        {
            _backupState = State;
            State = TileLevelMapState.Locked;
        }
    }
}