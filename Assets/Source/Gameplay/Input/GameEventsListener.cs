using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Input
{
	public abstract class GameEventsListener : MonoBehaviour
	{
		[Inject] private GameEventsHandler _gameEvents;

		void Awake()
		{
			_gameEvents.OnGameResumedAsObservable().Subscribe(OnGameResumed).AddTo(this);
			_gameEvents.OnGamePausedAsObservable().Subscribe(OnGamePaused).AddTo(this);
		}

		protected abstract void OnGamePaused(Unit obj);

		protected abstract void OnGameResumed(Unit obj);
	}
}