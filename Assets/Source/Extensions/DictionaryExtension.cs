using System.Collections.Generic;
using UniRx;

namespace TilesWalk.Extensions
{
	public static class DictionaryExtension
	{
		public static bool TryGetValue<T1, T2>(this Dictionary<T1, T2> dict, ReactiveProperty<T1> key, out T2 value)
		{
			return dict.TryGetValue(key.Value, out value);
		}
	}
}