using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Condition;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Level.UI
{
	/// <summary>
	/// This class deactivates the component <see cref="GameObject"/>
	/// if the map finish conditions matches
	/// </summary>
	public class DeactivateOnLevelMapFinishCondition : MonoBehaviour
	{
		[Inject] private TileViewLevelMap _tileLevelMap;

		/// <summary>
		/// The level finish conditions that the script checks for
		/// </summary>
		[SerializeField] private FinishCondition _condition;

		private void Awake()
		{
			_tileLevelMap
				.OnLevelMapDataLoadedAsObservable()
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