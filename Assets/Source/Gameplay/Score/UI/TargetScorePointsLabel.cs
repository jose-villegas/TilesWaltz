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
	public class TargetScorePointsLabel : ObligatoryComponentBehaviour<TextMeshProUGUI>
	{
		[Inject] private TileViewMap _tileMap;

		private void Start()
		{
			_tileMap
				.OnTileMapLoadedAsObservable()
				.Subscribe(
					tileMap => Component.text = tileMap.Target.ToString()
				)
				.AddTo(this);
		}
	}
}