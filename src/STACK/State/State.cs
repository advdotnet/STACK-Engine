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
            var Selector = new StackSurrogateSelector();

            return new BinaryFormatter(Selector, new StreamingContext())
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
            using (var Writer = new FileStream(filePath, FileMode.Create))
            {
                using (var ZipStream = new DeflateStream(Writer, CompressionMode.Compress))
                {
                    GetBinaryFormatter().Serialize(ZipStream, stateObject);
                }
            }
        }

        /// <summary>
        /// Decompresses and deserializes the content of a file into an object.
        /// </summary>
        public static T LoadFromFile<T>(string filePath)
        {
            using (var Reader = new FileStream(filePath, FileMode.Open))
            {
                using (var ZipStream = new DeflateStream(Reader, CompressionMode.Decompress))
                {
                    return (T)GetBinaryFormatter().Deserialize(ZipStream);
                }
            }
        }

        public static byte[] SaveState<T>(T stateObject)
        {
            using (var Writer = new MemoryStream())
            {
                GetBinaryFormatter().Serialize(Writer, stateObject);
                return Writer.ToArray();
            }
        }

        public static T LoadState<T>(byte[] bytes)
        {
            using (var Reader = new MemoryStream(bytes))
            {
                return (T)GetBinaryFormatter().Deserialize(Reader);
            }
        }
    }
}
