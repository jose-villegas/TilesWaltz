using System;
using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Condition;
using TilesWalk.General;
using TilesWalk.General.Patterns;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Building.LevelEditor.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class CustomLevelTimeTrackerLabel : ObligatoryComponentBehaviour<TextMeshProUGUI>
	{
		[Inject] private CustomLevelPlayer _customLevelPlayer;
		private TimeFinishCondition _condition;

		private void Start()
		{
			_condition = new TimeFinishCondition(Constants.CustomLevelName, float.MaxValue);

			_customLevelPlayer.OnPlayAsObservable().Subscribe(OnCustomLevelPlay).AddTo(this);
			_condition.Tracker
				.SubscribeToText(Component, val => new DateTime(TimeSpan.FromSeconds(val).Ticks).ToString(("mm:ss")))
				.AddTo(this);
		}

		private void OnCustomLevelPlay(LevelMap obj)
		{
			Component.text = "00:00";
			_condition.Reset(0);
		}
	}
}