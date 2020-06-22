﻿using System.Collections.Generic;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Condition;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Level.UI
{
	public class DeactivateOnLevelMapFinishCondition : MonoBehaviour
	{
		[Inject] private TileViewLevelMap _tileLevelMap;
		[SerializeField] private FinishCondition _condition;

		private void Start()
		{
			_tileLevelMap
				.OnLevelMapLoadedAsObservable()
				.Subscribe(
					OnLevelMapLoaded
				)
				.AddTo(this);
		}

		private void OnLevelMapLoaded(LevelMap levelMap)
		{
			if (levelMap.FinishCondition == _condition)
			{
				gameObject.SetActive(false);
			}
		}
	}
}