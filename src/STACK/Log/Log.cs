using System;
using System.Collections.Generic;

namespace STACK
{
    public interface ILogHandler
    {
        void WriteLine(string text, LogLevel level);
    }

    class DebugLogHandler : ILogHandler
    {
        public void WriteLine(string text, LogLevel level)
        {
            string prefix = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " ";
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

    class SystemConsoleLogHandler : ILogHandler
    {
        public void WriteLine(string text, LogLevel level)
        {
            string prefix = "";

            switch (level)
            {
                case LogLevel.Debug: prefix = " DEBUG: "; break;
                case LogLevel.Error: prefix = " ERROR: "; break;
                case LogLevel.Notice: prefix = " NOTICE: "; break;
                case LogLevel.Warning: prefix = " WARNING: "; break;
            }

            System.Console.WriteLine(prefix + text);
        }
    }

    public class Log
    {
        static readonly object Lock = new object();
        static readonly List<ILogHandler> LogHandler = new List<ILogHandler>();

        public static void WriteLine(string format, params object[] args)
        {
            WriteLine(String.Format(format, args));
        }

        public static void WriteLine(string text, LogLevel level = LogLevel.Notice)
        {
            lock (Lock)
            {
                foreach (ILogHandler Handler in LogHandler)
                {
                    Handler.WriteLine(text, level);
                }
            }
        }

        public static void AddLogger(ILogHandler logger)
        {
            if (logger == null)
            {
                throw new ArgumentException("Logger must not be null");
            }

            LogHandler.Add(logger);
        }

        public static T GetLogger<T>(Type type)
        {
            for (int i = 0; i < LogHandler.Count; i++)
            {
                if (LogHandler[i].GetType() == type)
                {
                    return (T)LogHandler[i];
                }
            }

            return default(T);
        }
    }

    public enum LogLevel
    {
        Notice, Warning, Error, Debug
    }
}
