using TilesWalk.Tile.Level;

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

            tile.gameObject.layer = gameObject.layer;
        }
    }
}