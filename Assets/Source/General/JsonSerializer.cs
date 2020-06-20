using System.IO;
using System.Text;
using BayatGames.SaveGameFree.Serializers;

namespace TilesWalk.General
{
	public class JsonSerializer : ISaveGameSerializer
	{
		public void Serialize<T>(T obj, Stream stream, Encoding encoding)
		{
			throw new System.NotImplementedException();
		}

		public T Deserialize<T>(Stream stream, Encoding encoding)
		{
			throw new System.NotImplementedException();
		}
	}
}