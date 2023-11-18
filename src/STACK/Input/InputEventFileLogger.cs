using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace STACK.Input
{
	/// <summary>
	/// Logs input events to a file for recording purposes.
	/// </summary>
	internal class InputEventFileLogger : IDisposable
	{
		private readonly Stream _fileStream;
		private readonly BinaryFormatter _formatter;
		private readonly DeflateStream _zipStream;

		public InputEventFileLogger(string filename)
		{
			_fileStream = File.Open(SaveGame.UserStorageFolder() + filename, FileMode.Create);
			_zipStream = new DeflateStream(_fileStream, CompressionMode.Compress, true);
			_formatter = new BinaryFormatter();
		}

		public void Log(InputEvent input)
		{
			_formatter.Serialize(_zipStream, input);
		}

		public void Dispose()
		{
			_zipStream.Flush();
			_zipStream.Close();
			_zipStream.Dispose();
			_fileStream.Dispose();
		}

		public void Flush()
		{
			Dispose();
		}
	}
}
