using System;
using System.IO;
using System.Text;
using BayatGames.SaveGameFree.Serializers;
using FullSerializer;
using Newtonsoft.Json;
using UnityEngine;

namespace TilesWalk.Gameplay.Persistence
{
	public class JsonSerializer : ISaveGameSerializer
	{
		public void Serialize<T>(T obj, Stream stream, Encoding encoding)
		{
#if !UNITY_WSA || !UNITY_WINRT
			try
			{
				StreamWriter writer = new StreamWriter(stream, encoding);
				writer.Write(JsonConvert.SerializeObject(obj));
				writer.Dispose();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
#else
			StreamWriter writer = new StreamWriter ( stream, encoding );
			writer.Write ( JsonUtility.ToJson ( obj ) );
			writer.Dispose ();
#endif
		}

		public T Deserialize<T>(Stream stream, Encoding encoding)
		{
			T result = default(T);
#if !UNITY_WSA || !UNITY_WINRT
			try
			{
				StreamReader reader = new StreamReader(stream, encoding);
				var content = reader.ReadToEnd();
				result = JsonConvert.DeserializeObject<T>(content);
				if (result == null)
				{
					result = default(T);
				}
				reader.Dispose();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
#else
			StreamReader reader = new StreamReader ( stream, encoding );
			result = JsonUtility.FromJson<T> ( reader.ReadToEnd () );
			reader.Dispose ();
#endif
			return result;
		}
	}
}