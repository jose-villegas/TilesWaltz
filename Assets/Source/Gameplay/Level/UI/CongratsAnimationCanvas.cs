using System;
using System.Collections;
using System.Linq;
using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Condition;
using TilesWalk.Gameplay.Persistence;
using TilesWalk.Gameplay.Score;
using TilesWalk.General.UI;
using TilesWalk.GPGS;
using TilesWalk.Map.General;
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
		[Inject] private TileViewLevelMap __tileLevelMap;
		[Inject] private ScorePointsConfiguration _scorePointsConfiguration;
		[Inject] private MapProviderSolver _solver;
		[Inject] private GameSave _gameSave;

		[Inject] private GPGSAchievementHandler _achievement;
		[Inject] private GPGSLeaderbardsHandler _leaderbards;

		[SerializeField] private LevelFinishDetailsCanvas _detailsCanvas;

		[Header("Points")] [SerializeField] private SlidingNumber _slidingNumber;
		[SerializeField] private Slider _slider;
		[Header("Actions")] [SerializeField] private Button _continue;
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
			// disabled interaction
			_retry.interactable = false;
			_continue.interactable = false;
			_summary.interactable = false;

			_slider.maxValue = __tileLevelMap.Map.Target;
			// initialize number at 0
			_slidingNumber.Current = 0;

			// update slider with number
			_slidingNumber.ObserveEveryValueChanged(x => x.Current).Subscribe(value =>
			{
				if (value <= __tileLevelMap.Map.Target)
				{
					var starCount = _gameScoresHelper.GetStarCount(__tileLevelMap.Map.Target, (int) value);

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
				var starCount = _gameScoresHelper.GetLastScoreStarCount(__tileLevelMap.Map);

				if (starCount < 3)
				{
					_animator.SetTrigger("LevelFailed");

					// restore interaction
					_retry.interactable = true;
					_continue.interactable = true;
					_summary.interactable = true;
					_levelScorePointsTracker.SaveScore();
					_gameSave.Statistics.MapFailed(_solver.Source);
					UpdateSocial();

					return;
				}

				_animator.SetTrigger("LevelCompleted");
				_gameSave.Statistics.MapCompleted(_solver.Source);

				switch (__tileLevelMap.Map.FinishCondition)
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

		private void UpdateSocial()
		{
			var collection = _solver.Provider.Collection;
			var records = _solver.Provider.Records;

			if (collection.AvailableMaps == null || collection.AvailableMaps.Count == 0) return;

			// check achievements
			switch (_solver.Source)
			{
				case Provider.GameMaps:
					var levelsCompleted = collection.AvailableMaps.Count(map => _gameScoresHelper.IsCompleted(map));
					_achievement.GameLevelCompletionAchievements(levelsCompleted);

					var totalScore = 0;

					foreach (var map in collection.AvailableMaps)
					{
						if (records.Exist(map.Id, out var score))
						{
							totalScore += score.Points.Highest;
						}
					}

					_leaderbards.ReportGameLevelsScore(totalScore);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private IEnumerator AnimateTimesExtra(TimeSpan last, TimeSpan limit)
		{
			var extra = Mathf.RoundToInt((float) (limit - last).TotalSeconds) *
			            _scorePointsConfiguration.PointsPerExtraSecond;

			yield return new WaitForSeconds(.1f);

			_animator.SetTrigger("ShowTimeExtra");

			var extraTime = (limit - last);
			_timeLabel.text = string.Format("{0:mm\\:ss}", extraTime);

			_timeSlidingNumber.ObserveEveryValueChanged(x => x.Current).Subscribe(value =>
			{
				var add = Mathf.Ceil(value / _scorePointsConfiguration.PointsPerExtraSecond);
				var seconds = TimeSpan.FromSeconds(add);
				var timeLeft = (extraTime - seconds);

				_timeLabel.text = string.Format("{0:mm\\:ss}", timeLeft);
			}).AddTo(this);

			_timeSlidingNumber.OnTargetReachedAsObservable().Subscribe(reached =>
			{
				Observable.Timer(TimeSpan.FromSeconds(1.5f))
					.Subscribe(_ => { }, () => _animator.SetTrigger("HideExtras"))
					.AddTo(this);
			}).AddTo(this);

			_slidingNumber.OnTargetReachedAsObservable().Subscribe(_ =>
			{
				// restore interaction
				_retry.interactable = true;
				_continue.interactable = true;
				_summary.interactable = true;
			}).AddTo(this);

			_timeSlidingNumber.Target(extra);
			_slidingNumber.Target(_currentScore.Points.Last);
			_levelScorePointsTracker.AddPoints(extra);
			_levelScorePointsTracker.SaveScore();
			UpdateSocial();

			yield return null;
		}

		private IEnumerator AnimateMovesExtra(int last, int limit)
		{
			var extra = ((limit - last) * _scorePointsConfiguration.PointsPerExtraMove);

			yield return new WaitForSeconds(.1f);

			_animator.SetTrigger("ShowMovesExtra");

			_movesLabel.text = $"{limit.Localize()}/{last.Localize()}";

			_movesSlidingNumber.ObserveEveryValueChanged(x => x.Current).Subscribe(value =>
			{
				var add = value / _scorePointsConfiguration.PointsPerExtraMove;
				_movesLabel.text = $"{(last + add).Localize()}/{limit.Localize()}";
			}).AddTo(this);

			_movesSlidingNumber.OnTargetReachedAsObservable().Subscribe(reached =>
			{
				Observable.Timer(TimeSpan.FromSeconds(1.5f))
					.Subscribe(_ => { }, () => _animator.SetTrigger("HideExtras"))
					.AddTo(this);
			}).AddTo(this);

			_slidingNumber.OnTargetReachedAsObservable().Subscribe(_ =>
			{
				// restore interaction
				_retry.interactable = true;
				_continue.interactable = true;
				_summary.interactable = true;
			}).AddTo(this);

			_levelScorePointsTracker.AddPoints(extra);
			_slidingNumber.Target(_currentScore.Points.Last);
			_movesSlidingNumber.Target(extra);
			_levelScorePointsTracker.SaveScore();
			UpdateSocial();

			yield return null;
		}
	}
}