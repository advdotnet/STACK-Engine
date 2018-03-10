using System.Collections.Generic;
using TomShane.Neoforce.Controls;

namespace STACK.Logging
{
    class ConsoleLogHandler : ILogHandler
    {
        Debug.Console _Console;
        List<ConsoleMessage> Buffer = new List<ConsoleMessage>();

        public ConsoleLogHandler(STACK.Debug.Console console)
        {
            _Console = console;
            Log.AddLogger(this);
        }

        public STACK.Debug.Console Console
        {
            get
            {
                return _Console;
            }

            set
            {
                _Console = value;
            }
        }

        public void WriteLine(string text, LogLevel level)
        {
            ConsoleMessage Message = new ConsoleMessage();
            Message.Text = text;

            switch (level)
            {
                case LogLevel.Debug:
                    Message.Channel = (byte)STACK.Debug.Console.Channel.Debug;
                    break;

                case LogLevel.Error:
                    Message.Channel = (byte)STACK.Debug.Console.Channel.Error;
                    break;

                case LogLevel.Notice:
                    Message.Channel = (byte)STACK.Debug.Console.Channel.Notice;
                    break;

                case LogLevel.Warning:
                    Message.Channel = (byte)STACK.Debug.Console.Channel.Warning;
                    break;
            }

            if (_Console == null)
            {
                Buffer.Add(Message);
            }
            else
            {
                if (Buffer.Count > 0)
                {
                    foreach (var Line in Buffer)
                    {
                        _Console.WriteLine(Line.Text, (STACK.Debug.Console.Channel)Line.Channel);
                    }

                    Buffer.Clear();
                }
                _Console.WriteLine(Message.Text, (STACK.Debug.Console.Channel)Message.Channel);
            }
        }
    }
}
