namespace STACK.Debug
{
	/// <summary>
	/// Stores a screenshot to the hard disk.
	/// </summary>
	internal class ScreenshotCommand : IConsoleCommand
	{
		public string Name => "screenshot";

		public string Description => "Makes a screenshot.";

#pragma warning disable IDE0052 // Ungelesene private Member entfernen
		private readonly StackEngine _engine;
#pragma warning restore IDE0052 // Ungelesene private Member entfernen

		public ScreenshotCommand(StackEngine game)
		{
			_engine = game;
		}

		public void Execute(Console console, string[] arguments)
		{
			//Engine.StackEngine.Renderer.SaveScreenShot(Engine.Game.World);
		}
	}
}
