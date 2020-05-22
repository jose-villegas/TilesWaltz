using TilesWalk.General;
using UnityEngine;

namespace TilesWalk.Extensions
{
	public static class Vector3IntExtension
	{
		public static Vector3Int forward()
		{
			return new Vector3Int(0, 0, 1);
		}

		public static Vector3Int backward()
		{
			return new Vector3Int(0, 0, -1);
		}
	}
}

