using System;
using TilesWalk.General;
using UnityEngine;
using UnityEngine.UI;

namespace TilesWalk.Building.LevelEditor.UI
{
	[Serializable]
	public class DirectionButton
	{
		[SerializeField] private CardinalDirection _direction;
		[SerializeField] private Button _button;

		public Button Button => _button;

		public CardinalDirection Direction => _direction;
	}
}