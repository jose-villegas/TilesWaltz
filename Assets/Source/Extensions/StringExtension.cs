using System;
using System.Globalization;

namespace TilesWalk.Extensions
{
	public static class StringExtension
	{
		public static string Localize(this int value)
		{
			return value.ToString("0,0", CultureInfo.GetCultureInfo("en-US"));
		}

		public static string Localize(this double value)
		{
			return value.ToString("0,0.00", CultureInfo.GetCultureInfo("en-US"));
		}

		public static string Localize(this float value)
		{
			return value.ToString("0,0.00", CultureInfo.GetCultureInfo("en-US"));
		}
	}
}