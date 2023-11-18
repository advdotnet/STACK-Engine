using System;

namespace STACK.Logging
{
	internal class DebugLogHandler : ILogHandler
	{
		public void WriteLine(string text, LogLevel level)
		{
			var prefix = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " ";
			switch (level)
			{
				case LogLevel.Debug: prefix += "DEBUG: "; break;
				case LogLevel.Error: prefix += "ERROR: "; break;
				case LogLevel.Notice: prefix += "NOTICE: "; break;
				case LogLevel.Warning: prefix += "WARNING: "; break;
			}

			System.Diagnostics.Debug.WriteLine(prefix + text);
		}
	}
}
