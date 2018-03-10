using System;
using System.Collections.Generic;
using System.Linq;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Input;

namespace STACK.Debug
{
    /// <summary>
    /// A debug console.
    /// </summary>
    public class Console
    {
        /// <summary>
        /// Gets the avaiable console commands.
        /// </summary>
        public IEnumerable<IConsoleCommand> Commands { get; private set; }

        ConsoleControl Control;
        StackEngine Engine;
        ConsoleHistory History = new ConsoleHistory();        

        /// <summary>
        /// Different channels used to categorize the console messages.
        /// </summary>
        public enum Channel : byte
        {
            System = 1,
            Debug = 2,
            Error = 3,
            Notice = 4,
            Warning = 5,
            User = 6
        }

        /// <summary>
        /// Returns if the console window is currently visible.
        /// </summary>
        public bool Visible
        {
            get
            {
                return Control.Visible;
            }
            set
            {
                Control.Visible = value;
            }
        }

        /// <summary>
        /// Gets or sets the current text input.
        /// </summary>
        public string Input
        {
            get
            {
                return Control.Input;
            }
            set
            {
                Control.Input = value;
            }
        }        

        public Console(StackEngine engine)
        {            
            Control = new ConsoleControl(engine, OnMessageSent, OnKeyUp);                        
            WriteLine("Welcome to STACK ENGINE");
            
            Commands = LoadCommands(engine);
            Engine = engine;
        }

        /// <summary>
        /// Writes a string to the console using given channel.
        /// </summary>
        /// <param name="message">The string to add.</param>
        /// <param name="channel">The channel to use.</param>
        public void WriteLine(object message, Console.Channel channel = Console.Channel.Notice)
        {
            Control.AddLine(message, channel);
        }

        /// <summary>
        /// Toggles the visibility.
        /// </summary>
        public void Toggle()
        {
            Visible = !Visible;
        }

        /// <summary>
        /// Uses reflection to create instances for each class implementing IConsoleCommand.
        /// </summary>
        IEnumerable<IConsoleCommand> LoadCommands(StackEngine engine)
        {
            var InterfaceType = typeof(IConsoleCommand);

            return AppDomain.CurrentDomain.GetAssemblies()
              .SelectMany(x => x.GetTypes())
              .Where(x => InterfaceType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
              .Select(x => (IConsoleCommand)Activator.CreateInstance(x, engine));            
        }

        /// <summary>
        /// Makes the up and down buttons cycle through the command history.
        /// </summary>
        void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Keys.Down)
            {    
                Control.Input = History.Next();
                Control.CursorPosition = Control.Input.Length;
            }
            else if (e.Key == Keys.Up)
            {
                Control.Input = History.Previous();
                Control.CursorPosition = Control.Input.Length;
            }
        }            

        void OnMessageSent(object sender, ConsoleMessageEventArgs e)
        {
            e.Handled = true;
            History.Add(e.Message.Text);
            Process(e.Message.Text);            
        }

        /// <summary>
        /// Processes a console command.
        /// </summary>
        /// <param name="buffer">The console command to execute.</param>
        void Process(string buffer)
        {
            var CommandName = GetCommandName(buffer);
            var Command = Commands.Where(c => c.Name == CommandName).FirstOrDefault();
            var Arguments = GetArguments(buffer);

            if (Command == null)
            {
                WriteLine("Command not found.", Console.Channel.Error);
            }
            else
            {
                try
                {
                    Command.Execute(this, Arguments);
                }
                catch (Exception ex)
                {
                    WriteLine("Error while executing command: " + ex.Message, Console.Channel.Error);
                }
            }
        }

        /// <summary>
        /// Returns the substring between the beginning of the sring and the first space.
        /// </summary>
        private string GetCommandName(string buffer)
        {
            var firstSpace = buffer.IndexOf(' ');

            return buffer.Substring(0, firstSpace < 0 ? buffer.Length : firstSpace);
        }

        /// <summary>
        /// Returns the parameters, seperated by spaces, as string array.
        /// </summary>
        private string[] GetArguments(string buffer)
        {
            var firstSpace = buffer.IndexOf(' ');

            if (firstSpace < 0)
            {
                return new string[0];
            }

            var args = buffer.Substring(firstSpace, buffer.Length - firstSpace).Split(' ');

            return args.Where(a => !string.IsNullOrEmpty(a)).ToArray();
        }
    }
}
