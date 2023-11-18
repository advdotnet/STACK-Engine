using System;
using System.Collections.Generic;

namespace STACK.Logging
{
	public class Log
	{
		private static readonly object _lock = new object();
		private static readonly List<ILogHandler> _logHandler = new List<ILogHandler>();

		public static void WriteLine(string format, params object[] args)
		{
			WriteLine(string.Format(format, args));
		}

		public static void WriteLine(string text, LogLevel level = LogLevel.Notice)
		{
			lock (_lock)
			{
				foreach (var handler in _logHandler)
				{
					handler.WriteLine(text, level);
				}
			}
		}

		public static void AddLogger(ILogHandler logger)
		{
			if (logger == null)
			{
				throw new ArgumentException("Logger must not be null.");
			}

			_logHandler.Add(logger);
		}

		public static T GetLogger<T>(Type type)
		{
			for (var i = 0; i < _logHandler.Count; i++)
			{
				if (_logHandler[i].GetType() == type)
				{
					return (T)_logHandler[i];
				}
			}

			return default;
		}
	}
}
