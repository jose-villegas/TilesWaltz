using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using TilesWalk.Extensions;
using TilesWalk.Gameplay.Installer;
using TilesWalk.General;
using TilesWalk.Map.General;
using UnityEngine;
using Zenject;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace TilesWalk.Map.Tile
{
	[RequireComponent(typeof(GameLevelTile))]
	public class GameLevelTileLinksHandler : MonoBehaviour
	{
		[Inject] private GameMapTileFactory _factory;
		[Inject] private GameLevelsMapBuilder _map;
		[Inject] private MapProviderSolver _solver;

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

		/// <summary>
		/// This method resolves the neighbor links for a game level tile in the game map.
		/// This is written in such way avoiding the costs of recursive iterations
		/// </summary>
		[Button(enabledMode: EButtonEnableMode.Editor)]
		public void ResolveLinks()
		{
			// since on editor mode dependencies cannot be solved, find the scripts
			if (_factory == null)
			{
				_factory = FindObjectOfType<GameMapTileFactory>();
			}

			// since on editor mode dependencies cannot be solved, find the scripts
			if (_map == null)
			{
				_map = FindObjectOfType<GameLevelsMapBuilder>();
			}

#if UNITY_EDITOR
			var maps = AssetDatabase.LoadAssetAtPath("Assets/Resources/GameMapsInstaller.asset",
				typeof(GameMapsInstaller)) as GameMapsInstaller;
			var mapCollection = maps.GameMap;
#else
			var mapCollection = _solver.Provider.Collection;
#endif

			var mapTile = GetComponentInParent<GameMapTile>();

			var linkOrigins =
				new Dictionary<CardinalDirection, List<KeyValuePair<CardinalDirection, TilesWalk.Tile.Tile>>>();

			foreach (var tileNeighbor in mapTile.Controller.Tile.Neighbors)
			{
				// this link is already solved
				if (_links != null && _links.Exists(x =>
					x.Direction == tileNeighbor.Key && x.Path != null && x.Path.Count > 0 && x.Level != null))
				{
					continue;
				}

				linkOrigins[tileNeighbor.Key] = new List<KeyValuePair<CardinalDirection, TilesWalk.Tile.Tile>>()
				{
					new KeyValuePair<CardinalDirection, TilesWalk.Tile.Tile>
					(
						tileNeighbor.Key,
						tileNeighbor.Value
					)
				};
			}

			bool hasNewRootsAvailable = true;

			while (hasNewRootsAvailable)
			{
				hasNewRootsAvailable = false;

				if (linkOrigins.Count > 0)
				{
					foreach (var pair in linkOrigins.ToList())
					{
						var roots = pair.Value;

						if (roots == null) continue;

						foreach (var root in roots)
						{
							LevelTileLink currentLink = null;

							if (_links == null) _links = new List<LevelTileLink>();

							var indexOf = _links.FindIndex(x => x.Direction == pair.Key);

							if (indexOf >= 0)
							{
								currentLink = _links[indexOf];
							}
							else
							{
								currentLink = new LevelTileLink(pair.Key);
								_links.Add(currentLink);
							}

							if (currentLink != null)
							{
								var gameTile = _map.GetTileView(root.Value);

								// we have reached the link for this direction, don't keep going deeper
								if (_map.IsLevelTile(root.Value) && currentLink.Level == null)
								{
									currentLink.Level = gameTile.GetComponentInChildren<GameLevelTile>();

									// set link on the other side for ease
									var linkHandler = currentLink.Level.GetComponent<GameLevelTileLinksHandler>();
									
									if (linkHandler._links == null)
									{
										linkHandler._links = new List<LevelTileLink>();
									}

									if (!linkHandler.Links.Exists(x => x.Direction == pair.Key.Opposite()))
									{
										var reverse = new List<GameObject>(currentLink.Path);
										reverse.Reverse();

										linkHandler.Links.Add(new LevelTileLink(pair.Key.Opposite())
										{
											Path = reverse, Level = GetComponent<GameLevelTile>()
										});
									}

									break;
								}

								// we are on a path, level link not reached, add more roots
								currentLink.Path.Add(gameTile.gameObject);

								// add new roots
								var newRoots = new List<KeyValuePair<CardinalDirection, TilesWalk.Tile.Tile>>();

								foreach (var neighbor in root.Value.Neighbors)
								{
									// avoid adding self, this would case a infinity loop
									if (neighbor.Key != root.Key.Opposite())
									{
										newRoots.Add(neighbor);
									}
								}

								if (newRoots.Count > 0)
								{
									linkOrigins[root.Key] = newRoots;
									hasNewRootsAvailable = true;
								}
							}
						}
					}
				}
			}
		}
	}
}