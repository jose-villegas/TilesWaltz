using System;
using System.Collections;
using System.Collections.Generic;
using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Condition;
using TilesWalk.Gameplay.Score;
using TilesWalk.General.Patterns;
using TilesWalk.General.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TilesWalk.Gameplay.Level.UI
{
	[RequireComponent(typeof(Animator))]
	public class CongratsAnimationCanvas : CanvasGroupBehaviour
	{
		[Inject] private LevelFinishTracker _levelFinishTracker;
		[Inject] private LevelScorePointsTracker _levelScorePointsTracker;
		[Inject] private GameScoresHelper _gameScoresHelper;
		[Inject] private TileViewLevelMap _levelMap;
		[Inject] private ScorePointsConfiguration _scorePointsConfiguration;

		[SerializeField] private LevelFinishDetailsCanvas _detailsCanvas;

		[Header("Points")]
		[SerializeField] private SlidingNumber _slidingNumber;
		[SerializeField] private Slider _slider;
		[SerializeField] private Button _retry;
		[SerializeField] private Button _summary;

		[Header("Conditions")] [SerializeField]
		private TextMeshProUGUI _movesLabel;

		[SerializeField] private SlidingNumber _movesSlidingNumber;
		[SerializeField] private TextMeshProUGUI _timeLabel;
		[SerializeField] private SlidingNumber _timeSlidingNumber;

		private Animator _animator;
		private LevelScore _currentScore;
		private bool _basePointsDone;

		// Start is called before the first frame update
		void Start()
		{
			_animator = GetComponent<Animator>();
			_levelFinishTracker.OnLevelFinishAsObservable().Subscribe(score =>
			{
				_currentScore = score;
				OnLevelFinish(_currentScore);

				_summary.onClick.AsObservable().Subscribe(_ => _levelScorePointsTracker.SaveScore()).AddTo(this);
				_retry.onClick.AsObservable().Subscribe(_ => _levelScorePointsTracker.SaveScore()).AddTo(this);

				_retry.onClick.AsObservable().Subscribe(_ => OnRetryClicked(_currentScore)).AddTo(this);
				_summary.onClick.AsObservable().Subscribe(_ => OnSummaryClicked(_currentScore)).AddTo(this);
			}).AddTo(this);
		}

		private void OnSummaryClicked(LevelScore score)
		{
			Hide();
			_detailsCanvas.OnLevelFinish(score);
		}

		private void OnRetryClicked(LevelScore score)
		{
		}

		private void OnLevelFinish(LevelScore score)
		{
			_slider.maxValue = _levelMap.LevelMap.Target;
			// initialize number at 0
			_slidingNumber.Component.text = 0.Localize();

			// update slider with number
			_slidingNumber.ObserveEveryValueChanged(x => x.Current).Subscribe(value =>
			{
				if (value <= _levelMap.LevelMap.Target)
				{
					var starCount = _gameScoresHelper.GetStarCount(_levelMap.LevelMap, (int) value);

					if (starCount >= 1)
					{
						_animator.SetInteger("Stars", starCount);
					}

					_slider.value = value;
				}
			}).AddTo(this);

			_slidingNumber.OnTargetReachedAsObservable().Subscribe(target =>
			{
				if (_basePointsDone)
				{
					return;
				}

				_basePointsDone = true;
				var starCount = _gameScoresHelper.GetStarCount(_levelMap.LevelMap, score.Points.Last);

				if (starCount < 3)
				{
					_animator.SetTrigger("LevelFailed");
					return;
				}

				_animator.SetTrigger("LevelCompleted");

				switch (_levelMap.LevelMap.FinishCondition)
				{
					case FinishCondition.TimeLimit:
						// check if we have extra time
						var timeLast = score.Time.Last;
						var timeLimit = _levelFinishTracker.TimeFinishCondition.Limit;
						StartCoroutine(AnimateTimesExtra(TimeSpan.FromSeconds(timeLast),
							TimeSpan.FromSeconds(timeLimit)));
						break;
					case FinishCondition.MovesLimit:
						// check if we have extra moves
						var movesLast = score.Moves.Last;
						var movesLimit = _levelFinishTracker.MovesFinishCondition.Limit;
						StartCoroutine(AnimateMovesExtra(movesLast, movesLimit));
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			});
			_slidingNumber.Target(Mathf.Min(score.Points.Last, _slider.maxValue));

			Show();
		}

		private IEnumerator AnimateTimesExtra(TimeSpan last, TimeSpan limit)
		{
			var extra = Mathf.RoundToInt((float) (limit - last).TotalSeconds) *
			            _scorePointsConfiguration.PointsPerExtraSecond;

			yield return new WaitForSeconds(.1f);

			_animator.SetTrigger("ShowTimeExtra");

			_timeLabel.text = $"{new DateTime(limit.Ticks):mm:ss} / {new DateTime(last.Ticks):mm:ss}";

			_timeSlidingNumber.ObserveEveryValueChanged(x => x.Current).Subscribe(value =>
			{
				var add = (int) value / _scorePointsConfiguration.PointsPerExtraSecond;
				_timeLabel.text =
					$"{new DateTime(new TimeSpan((int) last.TotalSeconds + add).Ticks):mm:ss} / " +
					$"{new DateTime(new TimeSpan((int) limit.TotalSeconds).Ticks):mm:ss}";
			}).AddTo(this);

			_timeSlidingNumber.OnTargetReachedAsObservable().Subscribe(reached =>
			{
				Observable.Timer(TimeSpan.FromSeconds(1.5f))
					.Subscribe(_ => { }, () => _animator.SetTrigger("HideExtras"))
					.AddTo(this);
			}).AddTo(this);

			_timeSlidingNumber.Target(extra);
			_slidingNumber.Target(_currentScore.Points.Last);
			_levelScorePointsTracker.AddPoints(extra);

			yield return null;
		}

		private IEnumerator AnimateMovesExtra(int last, int limit)
		{
			var extra = ((limit - last) * _scorePointsConfiguration.PointsPerExtraMove);

			yield return new WaitForSeconds(.1f);

			_animator.SetTrigger("ShowMovesExtra");

			_movesLabel.text = $"{limit.Localize()} / {last.Localize()}";

			_movesSlidingNumber.ObserveEveryValueChanged(x => x.Current).Subscribe(value =>
			{
				var add = value / _scorePointsConfiguration.PointsPerExtraMove;
				_movesLabel.text = $"{(last + add).Localize()} / {limit.Localize()}";
			}).AddTo(this);

			_movesSlidingNumber.OnTargetReachedAsObservable().Subscribe(reached =>
			{
				Observable.Timer(TimeSpan.FromSeconds(1.5f))
					.Subscribe(_ => {}, () => _animator.SetTrigger("HideExtras"))
					.AddTo(this);
			}).AddTo(this);

			_levelScorePointsTracker.AddPoints(extra);
			_slidingNumber.Target(_currentScore.Points.Last);
			_movesSlidingNumber.Target(extra);

			yield return null;
		}
	}
}