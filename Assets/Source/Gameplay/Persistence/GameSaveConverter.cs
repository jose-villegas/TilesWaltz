using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace TilesWalk.Gameplay.Persistence
{
	public static class GameSaveConverter
	{
		public static string Serialize<T>(T data)
		{
			return JsonConvert.SerializeObject(data);
		}

		public static T Deserialize<T>(string data)
		{
			return JsonConvert.DeserializeObject<T>(data);
		}
	}
}