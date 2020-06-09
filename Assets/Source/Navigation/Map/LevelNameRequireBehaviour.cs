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

		public ReactiveProperty<string> LevelName { get; set; } = new ReactiveProperty<string>();
		public TileMap TileMap { get; private set; }

		protected virtual void Start()
		{
			LevelName.Subscribe(level =>
			{
				if (string.IsNullOrEmpty(level)) return;

				TileMap = _availableMaps.Find(x => x.Id == level);

				if (TileMap != null)
				{
					OnTileMapFound();
				}
			}).AddTo(this);
		}

		protected abstract void OnTileMapFound();
	}
}