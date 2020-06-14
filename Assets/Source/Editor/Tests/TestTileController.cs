using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TilesWalk.Tile;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestTileController
    {
	    [Test]
	    public void TestAddingNeighbor()
	    {
			var controller = new TileController();
	    }


		/// <summary>
		/// Removal of a tile is not a removal in the complete sense
		/// what removal does it change the colors of all the tiles
		/// in the "removed" tile shortest path to leaf
		/// </summary>
	    [Test]
	    public void TestRemoveShuffleColor()
	    {
			var controller = new TileController();

	    }

    }
}
