using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace STACK.Input
{
    /// <summary>
    /// Logs input events to a file for recording purposes.
    /// </summary>
    class InputEventFileLogger : IDisposable
    {
        readonly Stream FileStream;
        readonly BinaryFormatter Formatter;
        readonly DeflateStream ZipStream;

        public InputEventFileLogger(string filename)
        {
            FileStream = File.Open(SaveGame.UserStorageFolder() + filename, FileMode.Create);
            ZipStream = new DeflateStream(FileStream, CompressionMode.Compress, true);
            Formatter = new BinaryFormatter();
        }

        public void Log(InputEvent input)
        {
            Formatter.Serialize(ZipStream, input);
        }

        public void Dispose()
        {
            ZipStream.Flush();
            ZipStream.Close();
            ZipStream.Dispose();
            FileStream.Dispose();
        }

        public void Flush()
        {
            Dispose();
        }
    }
}
