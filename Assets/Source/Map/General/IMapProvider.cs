using System.Collections.Generic;
using TilesWalk.Building.Level;

namespace TilesWalk.Map.General
{
    public interface IMapProvider
    {
        List<LevelMap> AvailableMaps { get; }
    }
}