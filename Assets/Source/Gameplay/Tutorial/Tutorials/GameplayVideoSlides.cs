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
        [SerializeField] private CanvasGroupBehaviour _slideContainer;
        [SerializeField] private VideoPlayer _videoPlayer;

        [Header("Content")] [SerializeField] private List<VideoClip> _clips;

        private Subject<int> _onNextButtonClick;
        private int _currentIndex;

        public VideoPlayer Player => _videoPlayer;

        public Button NextButton => _nextButton;

        public IObservable<int> OnNextButtonClickAsObservable()
        {
            return _onNextButtonClick = _onNextButtonClick == null ? new Subject<int>() : _onNextButtonClick;
        }

        protected override void RaiseOnCompletedOnDestroy()
        {
            _onNextButtonClick?.OnCompleted();
        }

        private void Awake()
        {
            _nextButton.onClick.AsObservable().Subscribe(OnNext);
        }

        private void Start()
        {
            var clip = _clips[_currentIndex];
            _videoPlayer.clip = clip;
            _videoPlayer.Prepare();
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
                _videoPlayer.Stop();
                _videoPlayer.clip = nextClip;
                _videoPlayer.Prepare();
                _slideContainer.Show();
            }).AddTo(this);

            _slideContainer.Hide();
        }
    }
}