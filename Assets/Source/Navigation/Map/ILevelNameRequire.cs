using UniRx;

namespace TilesWalk.Navigation.Map
{
	interface ILevelNameRequire
	{
		ReactiveProperty<string> LevelName { get; }
	}
}