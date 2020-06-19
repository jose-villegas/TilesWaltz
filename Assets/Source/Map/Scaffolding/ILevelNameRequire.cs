using TilesWalk.Building.Level;
using UniRx;

namespace TilesWalk.Map.Scaffolding
{
	interface ILevelNameRequire
	{
		ReactiveProperty<string> Name { get; }
		ReactiveProperty<LevelMap> Map { get; }
	}
}