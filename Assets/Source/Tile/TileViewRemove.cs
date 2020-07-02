using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using TilesWalk.Audio;
using TilesWalk.General;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Tile
{
	public partial class TileView
	{
		[Inject] private GameAudioCollection _audioCollection;

		private List<Tuple<Vector3, Quaternion>> ShufflePath(IReadOnlyList<TileView> shufflePath)
		{
			if (shufflePath == null || shufflePath.Count <= 0) return null;

			// this structure with backup the origin position and rotations
			var backup = new List<Tuple<Vector3, Quaternion>>();

			for (var i = 0; i < shufflePath.Count - 1; i++)
			{
				var source = shufflePath[i];
				var nextTo = shufflePath[i + 1];
				// backup info
				backup.Add(new Tuple<Vector3, Quaternion>(source.transform.position, source.transform.rotation));
				// copy transform
				source.transform.position = nextTo.transform.position;
				source.transform.rotation = nextTo.transform.rotation;
			}

			return backup;
		}

		[Button]
		private void Remove()
		{
			if (MovementLocked)
			{
				Debug.LogWarning(
					"Tile movement is currently locked, wait for unlock for removal to be available again");
				return;
			}

			MovementLocked = true;

			_controller.Remove();
			var shufflePath = _controller.Tile.ShortestPathToLeaf;

			if (shufflePath == null || shufflePath.Count <= 0) return;

			// play removal fx
			_particleSystems["Remove"].Play();

			var tiles = shufflePath.Select(x => _tileLevelMap.GetTileView(x)).ToList();
			// this structure with backup the origin position and rotations
			var backup = ShufflePath(tiles);

			// since the last tile has no other to exchange positions with, reduce its
			// scale to hide it before showing its new color
			var lastTile = _tileLevelMap.GetTileView(shufflePath[shufflePath.Count - 1]);
			var scale = lastTile.transform.localScale;
			lastTile.transform.localScale = Vector3.zero;

			_audioCollection.Play(GameAudioType.Sound, "Shuffle");
			MainThreadDispatcher.StartEndOfFrameMicroCoroutine(ShuffleMoveAnimation(tiles, backup));
			Observable.Timer(TimeSpan.FromSeconds(_animationSettings.ShuffleMoveTime))
				.DelayFrame(1)
				.Subscribe(_ => { }, () =>
				{
					lastTile._particleSystems["PopIn"].Play();
					MainThreadDispatcher.StartEndOfFrameMicroCoroutine(lastTile.ScalePopInAnimation(scale));
					Observable.Timer(TimeSpan.FromSeconds(_animationSettings.ScalePopInTime))
						.DelayFrame(1)
						.Subscribe(_ => { }, () =>
						{
							// finally the remove animation is done, check for power-ups
							if (_controller.Tile.PowerUp != TilePowerUp.None)
							{
								HandlePowerUp(() =>
								{
									MovementLocked = false;
									_onTileRemoved?.OnNext(shufflePath);
								});
							}
							else
							{
								MovementLocked = false;
								_onTileRemoved?.OnNext(shufflePath);
							}
						}).AddTo(this);
				}).AddTo(this);
		}

		private void HandlePowerUp(Action onFinish)
		{
			MovementLocked = true;
			List<Tile> path = null;
			var audioToPlay = "";
			var powerUp = _controller.Tile.PowerUp;

			switch (_controller.Tile.PowerUp)
			{
				case TilePowerUp.None:
					break;
				case TilePowerUp.NorthSouthLine:
					path = _controller.Tile.GetStraightPath(true, CardinalDirection.North, CardinalDirection.South);
					audioToPlay = "LinePower";
					break;
				case TilePowerUp.EastWestLine:
					path = _controller.Tile.GetStraightPath(true, CardinalDirection.East, CardinalDirection.West);
					audioToPlay = "LinePower";
					break;
				case TilePowerUp.ColorMatch:
					path = _controller.Tile.GetAllOfColor();
					audioToPlay = "ColorPower";
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			if (path != null)
			{
				for (int i = 0; i < path.Count; i++)
				{
					var index = i;
					var tileView = _tileLevelMap.GetTileView(path[i]);
					var sourceScale = tileView.transform.localScale;

					_audioCollection.Play(GameAudioType.Sound, audioToPlay);

					MainThreadDispatcher.StartEndOfFrameMicroCoroutine(
						tileView.ScalePopInAnimation(Vector3.zero));

					Observable.Timer(TimeSpan.FromSeconds(_animationSettings.ScalePopInTime))
						.DelayFrame(1)
						.Subscribe(_ => { }, () =>
						{
							if (index == path.Count - 1)
							{
								_controller.HandleTilePowerUp();
							}

							tileView._particleSystems["PopIn"].Play();
							MainThreadDispatcher.StartEndOfFrameMicroCoroutine(
								tileView.ScalePopInAnimation(sourceScale));
							Observable.Timer(TimeSpan.FromSeconds(_animationSettings.ScalePopInTime))
								.DelayFrame(1)
								.Subscribe(_ => { },
									() =>
									{
										if (index == path.Count - 1)
										{
											MovementLocked = false;
											_onPowerUpRemoval?.OnNext(new Tuple<List<Tile>, TilePowerUp>(path, powerUp));
											onFinish?.Invoke();
										}
									}).AddTo(this);
						}).AddTo(this);
				}
			}
		}

		[Button]
		private void RemoveCombo()
		{
			if (MovementLocked)
			{
				Debug.LogWarning(
					"Tile movement is currently locked, wait for unlock for removal to be available again");
				return;
			}

			// combo removals require at least three of the same color in the matching path
			if (_controller.Tile.MatchingColorPatch == null || _controller.Tile.MatchingColorPatch.Count <= 2)
			{
				Debug.LogWarning("A combo requires at least three matching color tiles together");
				return;
			}

			MovementLocked = true;
			var shufflePath = _controller.Tile.MatchingColorPatch;
			var indexOf = shufflePath.FindIndex(x => x.PowerUp != TilePowerUp.None);

			if (indexOf >= 0)
			{
				var view = _tileLevelMap.GetTileView(shufflePath[indexOf]);
				view.HandlePowerUp(() =>
				{
					MovementLocked = false;
					_onComboRemoval?.OnNext(shufflePath);
				});
				return;
			}

			for (int i = 0; i < shufflePath.Count; i++)
			{
				var index = i;
				var tileView = _tileLevelMap.GetTileView(shufflePath[i]);
				var sourceScale = tileView.transform.localScale;

				_audioCollection.Play(GameAudioType.Sound, "Combo");
				MainThreadDispatcher.StartEndOfFrameMicroCoroutine(tileView.ScalePopInAnimation(Vector3.zero));
				Observable.Timer(TimeSpan.FromSeconds(_animationSettings.ScalePopInTime))
					.DelayFrame(1)
					.Subscribe(_ => { }, () =>
					{
						if (index == shufflePath.Count - 1)
						{
							tileView.Controller.RemoveCombo();
						}

						tileView._particleSystems["PopIn"].Play();
						MainThreadDispatcher.StartEndOfFrameMicroCoroutine(tileView.ScalePopInAnimation(sourceScale));
						Observable.Timer(TimeSpan.FromSeconds(_animationSettings.ScalePopInTime))
							.DelayFrame(1)
							.Subscribe(_ => { },
								() =>
								{
									if (index == shufflePath.Count - 1)
									{
										MovementLocked = false;
										_onComboRemoval?.OnNext(shufflePath);
									}
								}).AddTo(this);
					}).AddTo(this);
			}
		}
	}
}