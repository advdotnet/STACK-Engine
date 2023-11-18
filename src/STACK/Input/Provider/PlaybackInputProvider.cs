using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace STACK.Input
{
	/// <summary>
	/// Reconstructs input events using a given recorded demo file.
	/// </summary>
	public class PlaybackInputProvider : InputProvider
	{
		private readonly Stream _reader;
		private readonly DeflateStream _zipStream;
		private readonly BinaryFormatter _formatter;
		private long _timeStamp;
		private readonly InputQueue _queue = new InputQueue();
		private bool _end = false;
		private InputEvent _currentEvent;

		public PlaybackInputProvider(string filename)
		{
			_reader = File.Open(filename, FileMode.Open, FileAccess.Read);
			_zipStream = new DeflateStream(_reader, CompressionMode.Decompress, true);
			_formatter = new BinaryFormatter();
			_currentEvent = LoadInputEventFromStream();
		}

		public override KeyboardState KeyboardState => throw new NotImplementedException();

		public override MouseState MouseState => throw new NotImplementedException();

		public InputEvent LoadInputEventFromStream()
		{
			var result = new InputEvent();

			try
			{
				result = (InputEvent)_formatter.Deserialize(_zipStream);
				_zipStream.Flush();
			}
			catch (System.Runtime.Serialization.SerializationException)
			{
				_end = true;
				_zipStream.Close();
				_zipStream.Dispose();
				_reader.Dispose();
			}

			return result;
		}

		private void Update()
		{
			_timeStamp += (long)GameSpeed.TickDuration;

			// make sure to enqueue all events for the current timestamp
			while (_currentEvent.Timestamp <= _timeStamp && !_end)
			{
				_queue.Enqueue(_currentEvent);
				_currentEvent = LoadInputEventFromStream();
			}
		}

		public override void Dispatch(bool paused)
		{
			Update();
			while (_queue.Count > 0)
			{
				var @event = _queue.Dequeue();
				@event.Paused = paused;
				Notify(@event);
			}
		}
	}
}
