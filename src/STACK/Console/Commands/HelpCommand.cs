using System.Linq;

namespace STACK.Debug
{
	/// <summary>
	/// Prints information about avaiable console commands.
	/// </summary>
	internal class HelpCommand : IConsoleCommand
	{
		public string Name => "help";

		public string Description => "Lists all avaiable commands. If a command name is given as a parameter, it shows help about that command.";

#pragma warning disable IDE0052 // Ungelesene private Member entfernen
		private readonly StackEngine _engine;
#pragma warning restore IDE0052 // Ungelesene private Member entfernen

		public HelpCommand(StackEngine game)
		{
			_engine = game;
		}

		public void Execute(Console console, string[] arguments)
		{
			if (arguments.Length == 0)
			{
				console.WriteLine("Avaiable Commands:");

				foreach (var command in console.Commands)
				{
					console.WriteLine(" " + command.Name);
				}
			}
			else if (arguments.Length == 1)
			{
				var command = console.Commands.Where(c => c.Name.ToUpperInvariant() == arguments[0].ToUpperInvariant()).FirstOrDefault();

				if (command == null)
				{
					console.WriteLine("Command not found.", Console.Channel.Error);
				}
				else
				{
					console.WriteLine(command.Description);
				}
			}
		}
	}
}
