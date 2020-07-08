using System;
using System.Collections.Generic;
using System.Linq;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Input;
using TilesWalk.Gameplay.Score;
using TilesWalk.General;
using TilesWalk.General.FX;
using TilesWalk.Map.Bridge;
using TilesWalk.Map.Scaffolding;
using TilesWalk.Navigation.UI;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace TilesWalk.Map.Tile
{
	public class GameLevelTile : ObservableTriggerBase, ILevelNameRequire
	{
		[Inject] private LevelMapDetailsCanvas _detailsCanvas;
		[Inject] private GameScoresHelper _gameScoresHelper;
		[Inject] private LevelStateTileMaterialHandler _colorHandler;
		[Inject] private LevelStarsTileMaterialHandler _starsColorHandler;
		[Inject] private GameEventsHandler _gameEvents;

		private MeshRenderer _meshRenderer;
		private ParticleSystemsCollector _particleSystems;
		private GameLevelTileLinksHandler _links;

		public ReactiveProperty<string> Name { get; } = new ReactiveProperty<string>();
		public ReactiveProperty<LevelMap> Map { get; } = new ReactiveProperty<LevelMap>();

		public GameLevelTileLinksHandler Links => _links;

		private Subject<GameLevelTile> _onLevelTileClick;
		private Subject<GameLevelTile> _onLevelDataLoaded;
		private bool _disabledClick;

		protected MeshRenderer Renderer
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

		private void OnMouseDown()
		{
			if (_disabledClick) return;

			OnMapTileClick();
		}

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

		public IObservable<GameLevelTile> OnLevelTileClickAsObservable()
		{
			return _onLevelTileClick = _onLevelTileClick ?? new Subject<GameLevelTile>();
		}

		public IObservable<GameLevelTile> OnLevelDataLoadedAsObservable()
		{
			return _onLevelDataLoaded = _onLevelDataLoaded ?? new Subject<GameLevelTile>();
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			_onLevelTileClick?.OnCompleted();
			_onLevelDataLoaded?.OnCompleted();
		}

		private void Awake()
		{
			_links = gameObject.GetComponent<GameLevelTileLinksHandler>();
			_particleSystems = gameObject.AddComponent<ParticleSystemsCollector>();
		}

		private void Start()
		{
			_gameEvents.OnGameResumedAsObservable().Subscribe(OnGameResumed).AddTo(this);
			_gameEvents.OnGamePausedAsObservable().Subscribe(OnGamePaused).AddTo(this);

			_detailsCanvas.LevelRequest.Name.Subscribe(mapName =>
			{
				if (mapName == Name.Value)
				{
					if (!_gameScoresHelper.IsCompleted(Map.Value))
					{
						_particleSystems["Completed"].Stop();
						_particleSystems["ToComplete"].Play();
					}
					else
					{
						_particleSystems["Completed"].Play();
						_particleSystems["ToComplete"].Stop();
					}
				}
				else
				{
					_particleSystems.StopAll();
				}
			}).AddTo(this);

			Map.Subscribe(map =>
			{
				if (map != null)
				{
					if (_gameScoresHelper.IsCompleted(map))
					{
						Renderer.material = _starsColorHandler.GetMaterial(Map.Value.StarsRequired);
					}
					else
					{
						if (_gameScoresHelper.GameStars < map.StarsRequired)
						{
							Renderer.material = _colorHandler.GetMaterial(LevelMapState.Locked);
						}
						else
						{
							Renderer.material = _colorHandler.GetMaterial(LevelMapState.ToComplete);
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