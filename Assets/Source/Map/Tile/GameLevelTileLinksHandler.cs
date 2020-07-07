using System.Collections.Generic;
using System.Linq;
using TilesWalk.General;
using UnityEngine;

namespace TilesWalk.Map.Tile
{
	[SerializeField]
	public class GameLevelTileLinksHandler : MonoBehaviour
	{
		[SerializeField] private List<LevelTileLink> _links;

		public List<LevelTileLink> Links => _links;

		public GameLevelTile this[CardinalDirection direction]
		{
			get { return _links.Find(x => x.Direction == direction).Level; }
		}

		public LevelTileLink GetLink(CardinalDirection direction)
		{
			if (_links == null) _links = new List<LevelTileLink>();

			if (_links.Count > 0)
			{
				var indexOf = _links.FindIndex(x => x.Direction == direction);

				if (indexOf >= 0)
				{
					return _links[indexOf];
				}

				_links.Add(new LevelTileLink(direction));
				return _links[_links.Count - 1];
			}

			_links.Add(new LevelTileLink(direction));
			return _links[_links.Count - 1];
		}

		/// <summary>
		/// Finds the link that has a matching endpoint within its path,
		/// if it doesn't exist it will add a link in the given direction
		/// </summary>
		/// <param name="endpoint"></param>
		/// <param name="direction"></param>
		/// <returns></returns>
		public LevelTileLink GetLink(GameObject endpoint, CardinalDirection direction)
		{
			if (_links == null) _links = new List<LevelTileLink>();

			if (_links.Count > 0)
			{
				var indexOf = _links.FindIndex(x => x.Path[x.Path.Count - 1] == endpoint);

				if (indexOf >= 0)
				{
					return _links[indexOf];
				}

				_links.Add(new LevelTileLink(direction));
				return _links[_links.Count - 1];
			}

			_links.Add(new LevelTileLink(direction));
			return _links[_links.Count - 1];
		}

		public bool HasNeighbor(CardinalDirection direction)
		{
			return _links.Any(x => x.Direction == direction);
		}
	}
}