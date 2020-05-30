using TilesWalk.Extensions;
using TMPro;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Score.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class CurrentScoreLabel : MonoBehaviour
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

		private void Start()
		{
			_scoreTracker.OnScoreUpdatedAsObservable().SubscribeToText(Label, score => score.LastScore.ToString());
		}
	}
}