using TilesWalk.Gameplay.Persistence;
using TilesWalk.Gameplay.Tutorial.Tutorials;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Zenject;

namespace TilesWalk.Gameplay.Tutorial
{
    [RequireComponent(typeof(Button))]
    public abstract class TutorialSequencePlayer : MonoBehaviour
    {
        [Inject] protected TutorialSequenceHandler _handler;
        [Inject] protected GameSave _save;
        [Inject] protected DiContainer _container;

        [SerializeField] protected TutorialVideoSlides _sliders;
        private TutorialVideoSlides _slidersInstance;

        protected TutorialVideoSlides SlidersInstance => _slidersInstance;

        private void Awake()
        {
            var button = GetComponent<Button>();

            button.onClick.AsObservable().Subscribe(unit => PlaySequence()).AddTo(this);
        }

        public abstract void PlaySequence();

        public abstract void FinishSequence();

        protected void InstanceTutorialVideoSlider(RectTransform content)
        {
            if (SlidersInstance == null)
            {
                var instance = _container.InstantiatePrefab(_sliders, content.transform);

                _slidersInstance = instance.GetComponent<TutorialVideoSlides>();
                _slidersInstance.Player.prepareCompleted += OnVideoPrepareCompleted;
            }
        }

        private void OnVideoPrepareCompleted(VideoPlayer source)
        {
            source.Play();
        }
    }
}