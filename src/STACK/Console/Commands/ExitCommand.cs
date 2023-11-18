namespace STACK.Debug
{
	/// <summary>
	/// Exits the game.
	/// </summary>
	internal class ExitCommand : IConsoleCommand
	{
		public string Name => "exit";

		public string Description => "Exits the game.";

		private readonly StackEngine _engine;

		public ExitCommand(StackEngine game)
		{
			_engine = game;
		}

		public void Execute(Console console, string[] arguments)
		{
			_engine.Exit();
		}
	}
}
