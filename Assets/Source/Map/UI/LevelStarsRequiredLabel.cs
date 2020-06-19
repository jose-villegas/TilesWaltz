using TilesWalk.Building.Level;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Score;
using TilesWalk.General.Patterns;
using TilesWalk.Map.Scaffolding;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Map.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class LevelStarsRequiredLabel : ObligatoryComponentBehaviour<TextMeshProUGUI>, ILevelNameRequire
	{
		[Inject] private GameScoresHelper _gameScoresHelper;

		public ReactiveProperty<string> Name { get; } = new ReactiveProperty<string>();
		public ReactiveProperty<LevelMap> Map { get; } = new ReactiveProperty<LevelMap>();

		private void Awake()
		{
			Map.SubscribeToText(Component,
				map => map != null ? $"{_gameScoresHelper.GameStars}/{map.StarsRequired}" : "").AddTo(this);
		}
	}
}