using System.Collections.Generic;
using TomShane.Neoforce.Controls;

namespace STACK.Logging
{
	internal class ConsoleLogHandler : ILogHandler
	{
		private Debug.Console _console;
		private readonly List<ConsoleMessage> _buffer = new List<ConsoleMessage>();

		public ConsoleLogHandler(Debug.Console console)
		{
			_console = console;
			Log.AddLogger(this);
		}

		public Debug.Console Console
		{
			get => _console;

			set => _console = value;
		}

		public void WriteLine(string text, LogLevel level)
		{
			var message = new ConsoleMessage
			{
				Text = text
			};

			switch (level)
			{
				case LogLevel.Debug:
					message.Channel = (byte)Debug.Console.Channel.Debug;
					break;

				case LogLevel.Error:
					message.Channel = (byte)Debug.Console.Channel.Error;
					break;

				case LogLevel.Notice:
					message.Channel = (byte)Debug.Console.Channel.Notice;
					break;

				case LogLevel.Warning:
					message.Channel = (byte)Debug.Console.Channel.Warning;
					break;
			}

			if (_console == null)
			{
				_buffer.Add(message);
			}
			else
			{
				if (_buffer.Count > 0)
				{
					foreach (var line in _buffer)
					{
						_console.WriteLine(line.Text, (Debug.Console.Channel)line.Channel);
					}

					_buffer.Clear();
				}
				_console.WriteLine(message.Text, (Debug.Console.Channel)message.Channel);
			}
		}
	}
}
