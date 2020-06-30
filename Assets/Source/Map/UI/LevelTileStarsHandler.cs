using System;
using System.Collections.Generic;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Score;
using TilesWalk.Gameplay.Score.Installer;
using TilesWalk.Map.Scaffolding;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TilesWalk.Navigation.UI
{
	public class LevelTileStarsHandler : MonoBehaviour, ILevelNameRequire
	{
		[Inject] private GameScoresHelper _gameScoresHelper;

		[SerializeField] private Sprite _starSprite;
		[SerializeField] private Sprite _starFilledSprite;
		[SerializeField] private Image[] _stars;

		public ReactiveProperty<string> Name { get; } =  new ReactiveProperty<string>();
		public ReactiveProperty<LevelMap> Map { get; } = new ReactiveProperty<LevelMap>();

		private void Start()
		{
			Map.Subscribe(OnTileMapUpdated).AddTo(this);
		}

		protected void OnTileMapUpdated(LevelMap levelMap)
		{
			if (levelMap == null) return;

			var stars = _gameScoresHelper.GetHighestScoreStarCount(levelMap);

			for (int i = 0; i < _stars.Length; i++)
			{
				_stars[i].sprite = _starSprite;

				if (stars >= i + 1)
				{
					_stars[i].sprite = _starFilledSprite;
				}
			}
		}

	}
}