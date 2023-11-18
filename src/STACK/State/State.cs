using STACK.Surrogates;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace STACK.State
{
	public class Serialization
	{
		/// <summary>
		/// Returns a BinaryFormatter wired up with the StackSurrogateSelector.
		/// </summary>        
		private static IFormatter GetBinaryFormatter()
		{
			var selector = new StackSurrogateSelector();

			return new BinaryFormatter(selector, new StreamingContext())
			{
				AssemblyFormat = FormatterAssemblyStyle.Simple,
				TypeFormat = FormatterTypeStyle.TypesWhenNeeded
			};
		}

		/// <summary>
		/// Serializes and compresses a given object and saves it to a file.
		/// </summary>
		public static void SaveToFile<T>(string filePath, T stateObject)
		{
			using (var writer = new FileStream(filePath, FileMode.Create))
			{
				using (var zipStream = new DeflateStream(writer, CompressionMode.Compress))
				{
					GetBinaryFormatter().Serialize(zipStream, stateObject);
				}
			}
		}

		/// <summary>
		/// Decompresses and deserializes the content of a file into an object.
		/// </summary>
		public static T LoadFromFile<T>(string filePath)
		{
			using (var reader = new FileStream(filePath, FileMode.Open))
			{
				using (var zipStream = new DeflateStream(reader, CompressionMode.Decompress))
				{
					return (T)GetBinaryFormatter().Deserialize(zipStream);
				}
			}
		}

		public static byte[] SaveState<T>(T stateObject)
		{
			using (var writer = new MemoryStream())
			{
				GetBinaryFormatter().Serialize(writer, stateObject);
				return writer.ToArray();
			}
		}

		public static T LoadState<T>(byte[] bytes)
		{
			using (var reader = new MemoryStream(bytes))
			{
				return (T)GetBinaryFormatter().Deserialize(reader);
			}
		}
	}
}
