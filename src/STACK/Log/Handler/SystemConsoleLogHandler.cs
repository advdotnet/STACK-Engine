namespace STACK.Logging
{
    public class SystemConsoleLogHandler : ILogHandler
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
}
