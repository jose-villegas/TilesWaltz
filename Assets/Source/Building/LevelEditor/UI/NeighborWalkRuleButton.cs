using System;
using TilesWalk.Tile.Rules;
using UnityEngine;
using UnityEngine.UI;

namespace TilesWalk.Building.LevelEditor.UI
{
	[Serializable]
	public class NeighborWalkRuleButton
	{
		[SerializeField] private NeighborWalkRule _rule;
		[SerializeField] private Toggle _toggle;

		public Toggle Toggle => _toggle;

		public NeighborWalkRule Rule => _rule;
	}
}