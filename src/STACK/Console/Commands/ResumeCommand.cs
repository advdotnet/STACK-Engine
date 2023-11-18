namespace STACK.Debug
{
	/// <summary>
	/// Resumes the game.
	/// </summary>
	internal class ResumeCommand : IConsoleCommand
	{
		public string Name => "resume";

		public string Description => "Resumes the game.";

		private readonly StackEngine _engine;

		public ResumeCommand(StackEngine game)
		{
			_engine = game;
		}

		public void Execute(Console console, string[] arguments)
		{
			if (_engine.Game != null && _engine.Game.World != null)
			{
				_engine.Game.World.Interactive = true;
			}
		}
	}
}
