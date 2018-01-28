using System;
using System.Collections.Generic;

namespace STACK.Logging
{
    public class Log
    {
        static readonly object Lock = new object();
        static readonly List<ILogHandler> LogHandler = new List<ILogHandler>();

        public static void WriteLine(string format, params object[] args)
        {
            WriteLine(string.Format(format, args));
        }

        public static void WriteLine(string text, LogLevel level = LogLevel.Notice)
        {
            lock (Lock)
            {
                foreach (var Handler in LogHandler)
                {
                    Handler.WriteLine(text, level);
                }
            }
        }

        public static void AddLogger(ILogHandler logger)
        {
            if (logger == null)
            {
                throw new ArgumentException("Logger must not be null.");
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
}
