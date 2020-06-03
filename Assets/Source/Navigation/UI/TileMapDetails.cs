using System.Collections.Generic;
using NaughtyAttributes;
using TilesWalk.Building.Map;
using TMPro;
using UnityEngine;
using Zenject;

namespace TilesWalk.Navigation.UI
{
	public class TileMapDetails : MonoBehaviour
	{
		[SerializeField] private bool _loadFromMapId;
		[SerializeField, ShowIf("_loadFromMapId")] private string _mapId;
		[SerializeField] private TextMeshProUGUI _name;
		[SerializeField] private TextMeshProUGUI _target;

		[Inject] private List<TileMap> _availableMaps;

		private void Start()
		{
			if (!_loadFromMapId) return;

			var found = _availableMaps.Find(x => x.Id == _mapId);

			if (found != null) LoadMapData(found);
		}

		public void LoadMapData(TileMap tileMap)
		{
			_name.text = tileMap.Id;
			_target.text = tileMap.Target.ToString();
		}
	}
}
