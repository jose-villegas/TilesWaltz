using UniRx;

namespace TilesWalk.Map.Scaffolding
{
	interface ILevelNameRequire
	{
		ReactiveProperty<string> LevelName { get; }
	}
}