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
	public class CustomLevelMovesCounterLabel : ObligatoryComponentBehaviour<TextMeshProUGUI>
	{
		[Inject] private CustomLevelPlayer _customLevelPlayer;
		private MovesFinishCondition _condition;

		private void Start()
		{
			_condition = new MovesFinishCondition(Constants.CustomLevelName, Int32.MaxValue);

			_customLevelPlayer.OnPlayAsObservable().Subscribe(OnCustomLevelPlay).AddTo(this);
			_condition.Tracker.SubscribeToText(Component, val => val.Localize()).AddTo(this);
		}

		private void OnCustomLevelPlay(LevelMap obj)
		{
			Component.text = 0.Localize();
			_condition.Reset(0);
		}
	}
}