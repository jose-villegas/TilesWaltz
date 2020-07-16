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

        private Subject<string> _onTextDialogCompleted;
        private Coroutine _running;
        private string _currentTarget;

        public IObservable<string> OnTextDialogCompletedAsObservable()
        {
            return _onTextDialogCompleted == null
                ? _onTextDialogCompleted = new Subject<string>()
                : _onTextDialogCompleted;
        }

        private void OnMouseDown()
        {
            if (_running != null)
            {
               StopAllCoroutines();
                _running = null;
            }

            Component.text = _currentTarget;
            _onTextDialogCompleted?.OnNext(_currentTarget);
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
                _onTextDialogCompleted?.OnNext(_currentTarget);

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

                Component.text += " ";
            }

            Component.text = text;
                
            yield return new WaitForSeconds(time);
            _onTextDialogCompleted?.OnNext(text);

            _running = null;
        }
    }
}