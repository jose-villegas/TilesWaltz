using UnityEngine;
using System.Collections;
using TilesWalk.Extensions;
using TilesWalk.General;
using TilesWalk.Tile;
using TilesWalk.Tile.Rules;

public static class TileViewExtension
{
	public static void InsertNeighbor(this TileView tileView, CardinalDirection direction, NeighborWalkRule rule,
		TileView neighbor)
	{
		tileView.Controller.AddNeighbor(direction, rule, neighbor.Controller.Tile,
			tileView.transform.localToWorldMatrix, out var translate, out var rotate);

		neighbor.transform.rotation = tileView.transform.rotation;
		neighbor.transform.Rotate(rotate.eulerAngles, Space.World);
		neighbor.transform.position = tileView.transform.position + translate;

		// join hinge points
		var src = tileView.Controller.Tile.HingePoints[direction];
		var dst = neighbor.Controller.Tile.HingePoints[direction.Opposite()];
		src = tileView.transform.position + tileView.transform.rotation * src;
		dst = neighbor.transform.position + neighbor.transform.rotation * dst;
		neighbor.transform.position += src - dst;
	}
}