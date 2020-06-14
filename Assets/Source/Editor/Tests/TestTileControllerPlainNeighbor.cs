using NUnit.Framework;
using TilesWalk.General;
using TilesWalk.Tile;
using TilesWalk.Tile.Rules;
using UnityEngine;

namespace Tests
{
	public class TestTileControllerPlainNeighbor
	{
		[Test]
		public void TestAddingNeighborNorthPlain()
		{
			var controller = new TileController();
			var model = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
			var tile = new Tile();
			controller.AddNeighbor(CardinalDirection.North, NeighborWalkRule.Plain, tile, model,
				out var translate, out var rotate);
			// reference is properly set
			Assert.AreSame(controller.Tile.Neighbors[CardinalDirection.North], tile);
			Assert.AreSame(tile.Neighbors[CardinalDirection.South], controller.Tile);
			// space adjustments are properly calculated
			Assert.IsTrue(controller.Tile.Index == Vector3.zero);
			Assert.IsTrue(tile.Index == Vector3.forward);
		}

		[Test]
		public void TestAddingNeighborSouthPlain()
		{
			var controller = new TileController();
			var model = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
			var tile = new Tile();
			controller.AddNeighbor(CardinalDirection.South, NeighborWalkRule.Plain, tile, model,
				out var translate, out var rotate);
			Assert.AreSame(controller.Tile.Neighbors[CardinalDirection.South], tile);
			Assert.AreSame(tile.Neighbors[CardinalDirection.North], controller.Tile);
			// space adjustments are properly calculated
			Assert.IsTrue(controller.Tile.Index == Vector3.zero);
			Assert.IsTrue(tile.Index == Vector3.back);
		}

		[Test]
		public void TestAddingNeighborEastPlain()
		{
			var controller = new TileController();
			var model = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
			var tile = new Tile();
			controller.AddNeighbor(CardinalDirection.East, NeighborWalkRule.Plain, tile, model,
				out var translate, out var rotate);
			Assert.AreSame(controller.Tile.Neighbors[CardinalDirection.East], tile);
			Assert.AreSame(tile.Neighbors[CardinalDirection.West], controller.Tile);
			// space adjustments are properly calculated
			Assert.IsTrue(controller.Tile.Index == Vector3.zero);
			Assert.IsTrue(tile.Index == Vector3.right);
		}

		[Test]
		public void TestAddingNeighborWestPlain()
		{
			var controller = new TileController();
			var model = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
			var tile = new Tile();
			controller.AddNeighbor(CardinalDirection.West, NeighborWalkRule.Plain, tile, model,
				out var translate, out var rotate);
			Assert.AreSame(controller.Tile.Neighbors[CardinalDirection.West], tile);
			Assert.AreSame(tile.Neighbors[CardinalDirection.East], controller.Tile);
			// space adjustments are properly calculated
			Assert.IsTrue(controller.Tile.Index == Vector3.zero);
			Assert.IsTrue(tile.Index == Vector3.left);
		}
	}
}