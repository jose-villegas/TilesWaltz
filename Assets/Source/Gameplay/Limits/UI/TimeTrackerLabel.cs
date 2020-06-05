using System;
using TilesWalk.Building.Map;
using TilesWalk.Navigation.UI;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Limits.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class TimeTrackerLabel : ObligatoryComponentBehaviour<TextMeshProUGUI>
	{
		[Inject] private TileViewMap _tileMap;
		private DateTime _start;

		private void Start()
		{
			_tileMap
				.OnTileMapLoadedAsObservable()
				.Subscribe(
					_ => { },
					() => _start = DateTime.Now
				)
				.AddTo(this);

			transform.UpdateAsObservable().Subscribe(_ =>
			{
				Component.text = new DateTime((DateTime.Now - _start).Ticks).ToString("mm:ss");
			}).AddTo(this);
		}
	}
}