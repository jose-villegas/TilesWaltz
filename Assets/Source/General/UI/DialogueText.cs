using System;
using System.Collections;
using TilesWalk.Gameplay.Animation;
using TilesWalk.General.Patterns;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.General.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class DialogueText : ObligatoryComponentBehaviour<TextMeshProUGUI>
    {
        [Inject] private AnimationConfiguration _animation;

        private Subject<string> _onTextDialogReadCompleted;
        private Subject<string> _onTextDialogFillCompleted;
        private Subject<string> _onWordCompleted;
        private Coroutine _running;
        private string _currentTarget;

        public IObservable<string> OnTextDialogFillCompletedAsObservable()
        {
            return _onTextDialogFillCompleted == null
                ? _onTextDialogFillCompleted = new Subject<string>()
                : _onTextDialogFillCompleted;
        }

        public IObservable<string> OnTextDialogReadCompletedAsObservable()
        {
            return _onTextDialogReadCompleted == null
                ? _onTextDialogReadCompleted = new Subject<string>()
                : _onTextDialogReadCompleted;
        }

        public IObservable<string> OnWordCompletedAsObservable()
        {
            return _onWordCompleted == null
                ? _onWordCompleted = new Subject<string>()
                : _onWordCompleted;
        }

        private void OnMouseDown()
        {
            if (_running != null)
            {
               StopAllCoroutines();
                _running = null;
            }

            Component.text = _currentTarget;
            _onTextDialogReadCompleted?.OnNext(_currentTarget);
        }

        public void ChangeText(string text, float time)
        {
            if (_running == null)
            {
                _currentTarget = text;
                _running = StartCoroutine(ChangeTextCoroutine(text, time));
            }
            else
            {
                StopAllCoroutines();
                _running = null;
                _currentTarget = text;
                _onTextDialogReadCompleted?.OnNext(_currentTarget);

                _running = StartCoroutine(ChangeTextCoroutine(text, time));
            }
        }

        private IEnumerator ChangeTextCoroutine(string text, float time)
        {
            Component.text = string.Empty;
            var timePerWord = 1f / _animation.WordsPerSecond;

            var split = text.Split(' ');

            for (int i = 0; i < split.Length; i++)
            {
                for (int j = 0; j < split[i].Length; j++)
                {
                    Component.text = Component.text + split[i][j];
                    yield return new WaitForSeconds(timePerWord / split[i].Length);
                }

                _onWordCompleted?.OnNext(split[i]);
                Component.text += " ";
            }

            Component.text = text;
            _onTextDialogFillCompleted?.OnNext(text);

            yield return new WaitForSeconds(time);
            _onTextDialogReadCompleted?.OnNext(text);

            _running = null;
        }
    }
}