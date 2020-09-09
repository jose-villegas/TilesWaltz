using TilesWalk.Tile.Level;
using UnityEngine;

namespace TilesWalk.Building.Level
{
    /// <summary>
    /// Used for background render and preview specifics
    /// </summary>
    public class TileViewLevelMapPreview : TileViewLevelMap
    {
        public override void RegisterTile(LevelTileView tile, int? hash = null)
        {
            base.RegisterTile(tile, hash);

            var children = tile.gameObject.GetComponentsInChildren<Transform>();

            foreach (var child in children)
            {
                child.gameObject.layer = gameObject.layer;
            }
        }
    }
}