using System.Collections.Generic;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Condition;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Level.UI
{
	public class ActiveOnLevelMapFinishCondition : MonoBehaviour
	{
		[Inject] private TileViewLevelMap _tileLevelMap;
		[SerializeField] private bool toggle = true;
		[SerializeField] private FinishCondition _condition;
		[SerializeField] private List<GameObject> _objects;

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
			if (levelMap.FinishCondition == _condition && _objects != null && _objects.Count > 0)
			{
				_objects.ForEach(x => x.SetActive(toggle));
			}
		}
	}
}