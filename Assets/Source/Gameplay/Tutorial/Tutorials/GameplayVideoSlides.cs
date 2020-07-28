using System;
using System.Collections.Generic;
using TilesWalk.General.UI;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace TilesWalk.Gameplay.Tutorial.Tutorials
{
    public class GameplayVideoSlides : ObservableTriggerBase
    {
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _previousButton;
        [SerializeField] private CanvasGroupBehaviour _slideContainer;
        [SerializeField] private VideoPlayer _videoPlayer;

        [Header("Content")] [SerializeField] private List<VideoClip> _clips;

        private Subject<int> _onNextButtonClick;
        private Subject<int> _onPreviousButtonClick;
        private int _currentIndex;

        public VideoPlayer Player => _videoPlayer;

        public IObservable<int> OnNextButtonClickAsObservable()
        {
            return _onNextButtonClick = _onNextButtonClick == null ? new Subject<int>() : _onNextButtonClick;
        }

        public IObservable<int> OnPreviousButtonClickAsObservable()
        {
            return _onPreviousButtonClick =
                _onPreviousButtonClick == null ? new Subject<int>() : _onPreviousButtonClick;
        }

        protected override void RaiseOnCompletedOnDestroy()
        {
            _onPreviousButtonClick?.OnCompleted();
            _onNextButtonClick?.OnCompleted();
        }

        private void Awake()
        {
            _nextButton.onClick.AsObservable().Subscribe(OnNext);
            _previousButton.onClick.AsObservable().Subscribe(OnPrevious);
        }

        private void Start()
        {
            var clip = _clips[_currentIndex];
            _videoPlayer.clip = clip;
        }

        private void OnPrevious(Unit unit)
        {
            _currentIndex -= 1;

            if (_currentIndex <= 0)
            {
                _previousButton.interactable = false;
            }
            else
            {
                _previousButton.interactable = true;
            }

            ShowNextClip();

            _onPreviousButtonClick?.OnNext(_currentIndex);
        }

        private void OnNext(Unit unit)
        {
            _currentIndex += 1;

            if (_currentIndex >= _clips.Count - 1)
            {
                _nextButton.interactable = false;
            }
            else
            {
                _nextButton.interactable = true;
            }

            ShowNextClip();

            _onNextButtonClick?.OnNext(_currentIndex);
        }

        private void ShowNextClip()
        {
            var nextClip = _clips[_currentIndex];

            _slideContainer.OnHideAsObservable().Take(1).Subscribe(unit1 =>
            {
                _videoPlayer.clip = nextClip;
                _slideContainer.Show();
            }).AddTo(this);

            _slideContainer.Hide();
        }
    }
}