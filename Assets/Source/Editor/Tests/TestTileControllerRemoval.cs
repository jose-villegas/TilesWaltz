using System.Linq;
using NUnit.Framework;
using TilesWalk.Gameplay.Display;
using TilesWalk.General;
using TilesWalk.Tile;
using TilesWalk.Tile.Rules;
using UnityEngine;

namespace Tests
{
	/// <summary>
	/// Tile removal doesn't work as a removal actually,
	/// instead it shuffles the colors from neighbor to neighbor
	/// in the shortest path to leaf for the removed tile
	/// </summary>
	public class TestTileControllerRemoval
	{
		[Test]
		public void TestTileRemovalShuffleColors()
		{
			var controller = new TileController[6];
			var model = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);

			// root
			controller[0] = new TileController();
			controller[0].AddNeighbor(CardinalDirection.North, NeighborWalkRule.Plain, new Tile(), model,
				out var translate, out var rotate);

			// neighbor
			controller[1] = new TileController(controller[0].Tile);
			controller[1].AddNeighbor(CardinalDirection.North, NeighborWalkRule.Plain, new Tile(), model,
				out translate, out rotate);

			// neighbor
			controller[2] = new TileController(controller[1].Tile);
			controller[2].AddNeighbor(CardinalDirection.North, NeighborWalkRule.Plain, new Tile(), model,
				out translate, out rotate);

			// neighbor
			controller[3] = new TileController(controller[2].Tile);
			controller[3].AddNeighbor(CardinalDirection.North, NeighborWalkRule.Plain, new Tile(), model,
				out translate, out rotate);

			// neighbor
			controller[4] = new TileController(controller[3].Tile);
			controller[4].AddNeighbor(CardinalDirection.North, NeighborWalkRule.Plain, new Tile(), model,
				out translate, out rotate);

			// neighbor
			controller[5] = new TileController(controller[4].Tile);
			controller[5].AddNeighbor(CardinalDirection.West, NeighborWalkRule.Plain, new Tile(), model,
				out translate, out rotate);

			var tiles = controller[5].Tile.ShortestPathToLeaf;
			var colors = tiles.Select(x => x.TileColor).ToList();
			controller[5].Remove();

			// check shuffle path
			for (int i = 0; i < tiles.Count - 1; i++)
			{
				var tile = tiles[i];
				Assert.IsTrue(tile.TileColor == colors[i + 1]);
			}
		}

		[Test]
		public void TestTileComboRemoval()
		{
			var controller = new TileController[6];
			var model = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);

			// root
			controller[0] = new TileController();
			controller[0].AddNeighbor(CardinalDirection.North, NeighborWalkRule.Plain, new Tile(), model,
				out var translate, out var rotate);

			// neighbor
			controller[1] = new TileController(controller[0].Tile.Neighbors[CardinalDirection.North]);
			controller[1].AddNeighbor(CardinalDirection.North, NeighborWalkRule.Plain, new Tile(), model,
				out translate, out rotate);

			// neighbor
			controller[2] = new TileController(controller[1].Tile.Neighbors[CardinalDirection.North]);
			controller[2].AddNeighbor(CardinalDirection.North, NeighborWalkRule.Plain, new Tile(), model,
				out translate, out rotate);

			// neighbor
			controller[3] = new TileController(controller[2].Tile.Neighbors[CardinalDirection.North]);
			controller[3].AddNeighbor(CardinalDirection.North, NeighborWalkRule.Plain, new Tile(), model,
				out translate, out rotate);
			controller[3].Tile.TileColor = TileColor.Red;

			// neighbor
			controller[4] = new TileController(controller[3].Tile.Neighbors[CardinalDirection.North]);
			controller[4].AddNeighbor(CardinalDirection.North, NeighborWalkRule.Plain, new Tile(), model,
				out translate, out rotate);
			controller[4].Tile.TileColor = TileColor.Red;

			// neighbor
			controller[5] = new TileController(controller[4].Tile.Neighbors[CardinalDirection.North]);
			controller[5].AddNeighbor(CardinalDirection.West, NeighborWalkRule.Plain, new Tile(), model,
				out translate, out rotate);
			controller[5].Tile.TileColor = TileColor.Red;

			TileController.ChainRefreshPaths(controller[5].Tile);

			// verify chain update
			Assert.IsTrue(controller[3].Tile.MatchingColorPatch.Count >= 3);
			Assert.IsTrue(controller[4].Tile.MatchingColorPatch.Count >= 3);
			Assert.IsTrue(controller[5].Tile.MatchingColorPatch.Count >= 3);

			var colorPath = controller[5].Tile.MatchingColorPatch;
			controller[5].RemoveCombo();

			// at least the first n-1 tiles should have a different color from source
			for (int i = 0; i < colorPath.Count - 1; i++)
			{
				Assert.IsTrue(colorPath[i].TileColor != TileColor.Red);
			}
		}


		[Test]
		public void TestTileColorShuffle()
		{
			var controller = new TileController();
			var model = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);

			// root
			controller = new TileController();
			controller.AddNeighbor(CardinalDirection.North, NeighborWalkRule.Plain, new Tile(), model,
				out var translate, out var rotate);
			controller.AddNeighbor(CardinalDirection.East, NeighborWalkRule.Plain, new Tile(), model,
				out translate, out rotate);
			controller.AddNeighbor(CardinalDirection.South, NeighborWalkRule.Plain, new Tile(), model,
				out translate, out rotate);
			controller.AddNeighbor(CardinalDirection.West, NeighborWalkRule.Plain, new Tile(), model,
				out translate, out rotate);

			var neighborColors = controller.Tile.Neighbors.Select(x => x.Value.TileColor);

			controller.Tile.ShuffleColor();

			Assert.IsTrue(!neighborColors.Contains(controller.Tile.TileColor));
		}
	}
}