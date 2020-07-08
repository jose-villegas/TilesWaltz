using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Score;
using TilesWalk.Map.Bridge;
using TilesWalk.Map.General;
using TilesWalk.Navigation.UI;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace TilesWalk.Map.Tile
{
	public class GameLevelTilesInitializer : ObservableTriggerBase
	{
		[Inject] private LevelMapDetailsCanvas _detailsCanvas;
		[Inject] private MapProviderSolver _solver;
		[Inject] private GameScoresHelper _gameScoresHelper;
		[Inject] private LevelBridge _bridge;
		[Inject] private LevelStateTileMaterialHandler _colorHandler;
		[Inject] private LevelStarsTileMaterialHandler _starsColorHandler;

		private ReactiveProperty<int> _readyCount = new ReactiveProperty<int>();

		private Subject<List<GameLevelTile>> _levelTilesMapsReady;

		public List<GameLevelTile> LevelTiles { get; private set; }

		public GameLevelTile this[LevelMap map]
		{
			get { return LevelTiles.FirstOrDefault(x => x.Map.Value.Id == map.Id); }
		}

		public GameLevelTile this[int i] => LevelTiles[i];

		private void Awake()
		{
			_solver.InstanceProvider(gameObject);

			var inChildren = GetComponentsInChildren<GameLevelTile>();
			var readyAt = inChildren.Length;
			LevelTiles = inChildren.ToList();

			foreach (var gameLevelTile in LevelTiles)
			{
				gameLevelTile.OnLevelDataLoadedAsObservable().Subscribe(levelTile => { _readyCount.Value += 1; });
			}

			_readyCount.Subscribe(count =>
			{
				if (count != readyAt) return;

				LevelTiles.Sort((p1, p2) => p1.Map.Value.StarsRequired - p2.Map.Value.StarsRequired);
				_levelTilesMapsReady?.OnNext(LevelTiles);

				ShowNextLevelDetails();
				HandleAnimations();
			}).AddTo(this);
		}

		private void HandleAnimations()
		{
			HashSet<Animator> blocked = new HashSet<Animator>();
			HashSet<Animator> incomplete = new HashSet<Animator>();
			HashSet<Animator> justUnlocked = new HashSet<Animator>();
			HashSet<Animator> completed = new HashSet<Animator>();
			Dictionary<Animator, int> maxStars = new Dictionary<Animator, int>();
			List<KeyValuePair<GameLevelTile, GameLevelTile>> linksDone =
				new List<KeyValuePair<GameLevelTile, GameLevelTile>>();

			foreach (var levelTile in LevelTiles)
			{
				var links = levelTile.Links.Links;

				// no linking for this tile
				if (links == null || links.Count == 0) continue;

				var sourceState = _gameScoresHelper.State(levelTile.Map.Value);

				foreach (var levelTileLink in links)
				{
					// check if this link is already done
					if (linksDone.Any((kv) =>
						kv.Value == levelTile && kv.Key == levelTileLink.Level ||
						kv.Key == levelTile && kv.Value == levelTileLink.Level))
					{
						continue;
					}

					var linkState = _gameScoresHelper.State(levelTileLink.Level.Map.Value);

					// from locked to locked
					if (linkState == LevelMapState.Locked || sourceState == LevelMapState.Locked)
					{
						foreach (var o in levelTileLink.Path)
						{
							if (blocked.Add(o.GetComponent<Animator>()))
							{
								// get model renderer
								var meshRenderer = o.GetComponentInChildren<MeshRenderer>();
								meshRenderer.material = _colorHandler.GetMaterial(LevelMapState.Locked);
							}
						}
					}

					if (sourceState == LevelMapState.Completed && linkState == LevelMapState.Completed)
					{
						foreach (var o in levelTileLink.Path)
						{
							if (completed.Add(o.GetComponent<Animator>()))
							{
								// get model renderer
								var meshRenderer = o.GetComponentInChildren<MeshRenderer>();
								meshRenderer.material = _starsColorHandler.GetMaterial(Mathf.Max(
									levelTile.Map.Value.StarsRequired,
									levelTileLink.Level.Map.Value.StarsRequired));
							}
						}
					}

					// from incomplete to incomplete
					if (sourceState == LevelMapState.ToComplete && linkState == LevelMapState.ToComplete ||
					    sourceState == LevelMapState.Completed && linkState == LevelMapState.ToComplete)
					{
						// check if this tile was just completed
						if (_bridge.Payload != null &&
						    _bridge.Payload.State == LevelMapState.ToComplete &&
						    levelTile.Map.Value.Id == _bridge.Payload.Level.Id)
						{
							foreach (var o in levelTileLink.Path)
							{
								if (justUnlocked.Add(o.GetComponent<Animator>()))
								{
									// get model renderer
									var meshRenderer = o.GetComponentInChildren<MeshRenderer>();
									meshRenderer.material = _colorHandler.GetMaterial(LevelMapState.Locked);

									maxStars[justUnlocked.Last()] = Mathf.Max(
										levelTile.Map.Value.StarsRequired,
										levelTileLink.Level.Map.Value.StarsRequired);
								}
							}
						}
						else
						{
							foreach (var o in levelTileLink.Path)
							{
								if (incomplete.Add(o.GetComponent<Animator>()))
								{
									// get model renderer
									var meshRenderer = o.GetComponentInChildren<MeshRenderer>();
									meshRenderer.material = _starsColorHandler.GetMaterial(
										Mathf.Max(levelTile.Map.Value.StarsRequired,
											levelTileLink.Level.Map.Value.StarsRequired));
								}
							}
						}
					}

					linksDone.Add(new KeyValuePair<GameLevelTile, GameLevelTile>(levelTile, levelTileLink.Level));
				}
			}


			for (int i = 0; i < blocked.Count; i++)
			{
				var animator = blocked.ElementAt(i);
				animator.SetFloat("Offset", (float) (i + 1) / blocked.Count);
				animator.SetBool("IsBlocked", true);
			}

			for (int i = 0; i < justUnlocked.Count; i++)
			{
				var animator = justUnlocked.ElementAt(i);
				var pos = animator.transform.position;
				var rot = animator.transform.rotation;
				var sca = animator.transform.localScale;

				animator.SetFloat("Offset", (float) (i + 1) / justUnlocked.Count);
				animator.SetBool("IsBlocked", true);

				// first stop blocked path anim
				Observable.Timer(TimeSpan.FromSeconds(.1f)).Subscribe(_ => { },
					() => { animator.SetBool("IsBlocked", false); }).AddTo(this);
				// now animate appear
				var time = .1f + (i + 1) * .3f;
				Observable.Timer(TimeSpan.FromSeconds(time)).Subscribe(_ => { },
					() =>
					{
						{
							animator.SetTrigger("IsNowAvailable");
							var meshRenderer = animator.GetComponentInChildren<MeshRenderer>();
							meshRenderer.material = _starsColorHandler.GetMaterial(maxStars[animator]);
						}
					}).AddTo(this);

				// restore to original transform
				Observable.Timer(TimeSpan.FromSeconds(time + .5f)).Subscribe(_ => { },
					() =>
					{
						animator.enabled = false;
						StartCoroutine(RestoreTransform(animator.transform, pos, rot, sca));
					}).AddTo(this);
			}
		}

		private IEnumerator RestoreTransform(Transform tr, Vector3 pos, Quaternion rot, Vector3 sca)
		{
			var t = 0f;

			var srPos = tr.position;
			var srRot = tr.rotation;
			var srSca = tr.localScale;
			var time = .5f;

			while (t <= time)
			{
				tr.position = Vector3.Lerp(srPos, pos, t / time);
				tr.rotation = Quaternion.Slerp(srRot, rot, t / time);
				tr.localScale = Vector3.Lerp(srSca, sca, t / time);

				t += Time.deltaTime;
				yield return null;
			}

			tr.position = pos;
			tr.rotation = rot;
			tr.localScale = sca;
			yield return null;
		}

		private void ShowNextLevelDetails()
		{
			// check if we are coming from a recently played game
			if (_bridge.Payload != null)
			{
				ShowPayloadLevel(_bridge.Payload.Level);
			}
			else
			{
				foreach (var level in LevelTiles)
				{
					if (_gameScoresHelper.GameStars < level.Map.Value.StarsRequired) continue;

					if (_solver.Provider.Records.Exist(level.Name.Value, out var score))
					{
						if (score.Points.Highest < level.Map.Value.Target)
						{
							_detailsCanvas.LevelRequest.Name.Value = level.Map.Value.Id;
							_detailsCanvas.Show();
							return;
						}
					}
					else
					{
						_detailsCanvas.LevelRequest.Name.Value = level.Map.Value.Id;
						_detailsCanvas.Show();
						return;
					}
				}

				// no next map found
				_detailsCanvas.LevelRequest.Name.Value = LevelTiles[0].Map.Value.Id;
				_detailsCanvas.Show();
			}
		}

		private void ShowPayloadLevel(LevelMap payloadLevel)
		{
			if (_solver.Provider.Records.Exist(payloadLevel.Id, out var score))
			{
				_detailsCanvas.LevelRequest.Name.Value = payloadLevel.Id;
				_detailsCanvas.Show();
			}
		}

		protected override void RaiseOnCompletedOnDestroy()
		{
			_levelTilesMapsReady?.OnCompleted();
		}

		public IObservable<List<GameLevelTile>> OnLevelTilesMapsReadyAsObservable()
		{
			return _levelTilesMapsReady = _levelTilesMapsReady ?? new Subject<List<GameLevelTile>>();
		}
	}
}