using System.Collections.Generic;
using TilesWalk.Building.Level;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Navigation.Map
{
	public abstract class LevelNameRequireBehaviour : MonoBehaviour, ILevelNameRequire
	{
		[Inject] private List<TileMap> _availableMaps;

		public ReactiveProperty<string> LevelName { get; } = new ReactiveProperty<string>();
		public ReactiveProperty<TileMap> TileMap { get; } = new ReactiveProperty<TileMap>();

		public List<TileMap> AvailableMaps => _availableMaps;

		protected virtual void Start()
		{
			LevelName.Subscribe(level =>
			{
				if (string.IsNullOrEmpty(level)) return;

				TileMap.Value = _availableMaps.Find(x => x.Id == level);
			}).AddTo(this);
		}
	}
}