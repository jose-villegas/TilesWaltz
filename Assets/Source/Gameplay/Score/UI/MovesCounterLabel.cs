using TilesWalk.Building.Map;
using TilesWalk.Extensions;
using TilesWalk.Navigation.UI;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace TilesWalk.Gameplay.Score.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class MovesCounterLabel : ObligatoryComponentBehaviour<TextMeshProUGUI>
	{
		[Inject] private TileViewMap _tileMap;
		private int _counter;

		private void Start()
		{
			_tileMap
				.OnTileRemovedAsObservable()
				.SubscribeToText(Component, _ =>
				{
					_counter++;
					return _counter.ToString();
				})
				.AddTo(this);
		}
	}
}