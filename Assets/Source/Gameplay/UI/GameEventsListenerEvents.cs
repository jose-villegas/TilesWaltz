using System;
using TilesWalk.Gameplay.Input;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace TilesWalk.Gameplay.UI
{
	public class GameEventsListenerEvents : GameEventsListener
	{
		[SerializeField] private UnityEvent _onGamePaused;
		[SerializeField] private UnityEvent _onGameResumed;

		protected override void OnGamePaused(Unit obj)
		{
			_onGamePaused?.Invoke();
		}

		protected override void OnGameResumed(Unit obj)
		{
			_onGameResumed?.Invoke();
		}
	}
}
