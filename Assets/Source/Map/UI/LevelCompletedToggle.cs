using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Score;
using TilesWalk.General.Patterns;
using TilesWalk.Map.Scaffolding;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TilesWalk.Map.UI
{
	[RequireComponent(typeof(Toggle))]
	public class LevelCompletedToggle : ObligatoryComponentBehaviour<Toggle>, ILevelNameRequire
	{
		[Inject] private GameScoresHelper _gameScoresHelper;

		public ReactiveProperty<string> Name { get; } = new ReactiveProperty<string>();
		public ReactiveProperty<LevelMap> Map { get; } = new ReactiveProperty<LevelMap>();

		private void Awake()
		{
			Map.Subscribe(map => Component.isOn = map != null && _gameScoresHelper.IsCompleted(map)).AddTo(this);
		}
	}
}