using System.Collections.Generic;
using TilesWalk.General.UI;
using UniRx;
using UnityEngine;
using UnityEngine.Video;

namespace TilesWalk.Gameplay.Tutorial.Tutorials
{
    public class TutorialVideoSlides : MonoBehaviour
    {
        [SerializeField] private CanvasGroupBehaviour _slideContainer;
        [SerializeField] private VideoPlayer _videoPlayer;

        [Header("Content")] [SerializeField] private List<VideoClip> _clips;

        private int _currentIndex;

        public VideoPlayer Player => _videoPlayer;

        private void Awake()
        {
        }

        private void Start()
        {
            var clip = _clips[_currentIndex];
            _videoPlayer.clip = clip;
            _videoPlayer.Prepare();
        }

        public void NextClip()
        {
            _currentIndex += 1;

            if (_currentIndex >= _clips.Count)
            {
                return;
            }

            ShowNextClip();
        }

        public void PreviousClip()
        {
            _currentIndex -= 1;

            if (_currentIndex < 0)
            {
                return;
            }

            ShowNextClip();
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