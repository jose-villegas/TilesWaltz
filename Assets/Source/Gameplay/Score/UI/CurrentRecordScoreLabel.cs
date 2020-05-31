using TilesWalk.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Score.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class CurrentRecordScoreLabel : MonoBehaviour
	{
		[Inject] private ScoreTracker _scoreTracker;

		private TextMeshProUGUI _label;

		public TextMeshProUGUI Label
		{
			get
			{
				if (_label == null)
				{
					_label = GetComponent<TextMeshProUGUI>();

					if (_label == null)
					{
						_label = gameObject.AddComponent<TextMeshProUGUI>();
					}
				}

				return _label;
			}
		}

		private void Awake()
		{
			_scoreTracker
				.OnScoresLoadedAsObservable()
				.Subscribe(
					_ => { },
					() => Label.text = _scoreTracker.ActiveScore.HighestScore.ToString()
				)
				.AddTo(this);

			_scoreTracker
				.OnScoreUpdatedAsObservable()
				.SubscribeToText(Label, score => score.HighestScore.ToString())
				.AddTo(this);
		}
	}
}