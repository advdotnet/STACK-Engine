using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using TomShane.Neoforce.Controls;

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

		private readonly ConsoleControl _control;
#pragma warning disable IDE0052 // Ungelesene private Member entfernen
		private readonly StackEngine _engine;
#pragma warning restore IDE0052 // Ungelesene private Member entfernen
		private readonly ConsoleHistory _history = new ConsoleHistory();

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
			get => _control.Visible;
			set => _control.Visible = value;
		}

		/// <summary>
		/// Gets or sets the current text input.
		/// </summary>
		public string Input
		{
			get => _control.Input;
			set => _control.Input = value;
		}

		public Console(StackEngine engine)
		{
			_control = new ConsoleControl(engine, OnMessageSent, OnKeyUp);
			WriteLine("Welcome to STACK ENGINE");

			Commands = LoadCommands(engine);
			_engine = engine;
		}

		/// <summary>
		/// Writes a string to the console using given channel.
		/// </summary>
		/// <param name="message">The string to add.</param>
		/// <param name="channel">The channel to use.</param>
		public void WriteLine(object message, Channel channel = Channel.Notice)
		{
			_control.AddLine(message, channel);
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
		private IEnumerable<IConsoleCommand> LoadCommands(StackEngine engine)
		{
			var interfaceType = typeof(IConsoleCommand);

			return AppDomain.CurrentDomain.GetAssemblies()
			  .SelectMany(x => x.GetTypes())
			  .Where(x => interfaceType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
			  .Select(x => (IConsoleCommand)Activator.CreateInstance(x, engine));
		}

		/// <summary>
		/// Makes the up and down buttons cycle through the command history.
		/// </summary>
		private void OnKeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Keys.Down)
			{
				_control.Input = _history.Next();
				_control.CursorPosition = _control.Input.Length;
			}
			else if (e.Key == Keys.Up)
			{
				_control.Input = _history.Previous();
				_control.CursorPosition = _control.Input.Length;
			}
		}

		private void OnMessageSent(object sender, ConsoleMessageEventArgs e)
		{
			e.Handled = true;
			_history.Add(e.Message.Text);
			Process(e.Message.Text);
		}

		/// <summary>
		/// Processes a console command.
		/// </summary>
		/// <param name="buffer">The console command to execute.</param>
		private void Process(string buffer)
		{
			var commandName = GetCommandName(buffer);
			var command = Commands.Where(c => c.Name == commandName).FirstOrDefault();
			var arguments = GetArguments(buffer);

			if (command == null)
			{
				WriteLine("Command not found.", Channel.Error);
			}
			else
			{
				try
				{
					command.Execute(this, arguments);
				}
				catch (Exception ex)
				{
					WriteLine("Error while executing command: " + ex.Message, Channel.Error);
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
