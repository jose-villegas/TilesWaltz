using System;
using UnityEngine;

namespace TilesWalk.Building.Level
{
	[Serializable]
	public class RootTile
	{
		public int Key;
		public Vector3 Position;
		public Vector3 Rotation;

		public RootTile()
		{
			Key = 0;
			Position = Vector3.zero;
			Rotation = Vector3.zero;
		}
	}
}