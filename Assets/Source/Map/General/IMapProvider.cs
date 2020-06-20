using System.Collections.Generic;
using TilesWalk.Building.Level;
using TilesWalk.Gameplay.Persistence;

namespace TilesWalk.Map.General
{
    public interface IMapProvider
    {
        GameMapCollection Collection { get; }
    }
}