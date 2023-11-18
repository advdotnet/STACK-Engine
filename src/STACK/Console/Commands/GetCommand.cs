namespace STACK.Debug
{
	/// <summary>
	/// Gets the value of a variable.
	/// </summary>
	internal class GetCommand : IConsoleCommand
	{
		public string Name => "get";

		public string Description => "Gets a variable.";

#pragma warning disable IDE0052 // Ungelesene private Member entfernen
		private readonly StackEngine _engine;
#pragma warning restore IDE0052 // Ungelesene private Member entfernen

		public GetCommand(StackEngine game)
		{
			_engine = game;
		}

		public void Execute(Console console, string[] arguments)
		{
			if (arguments.Length == 1)
			{
				var props = typeof(EngineVariables).GetFields();

				if (arguments[0].ToUpperInvariant() == "STACK")
				{
					foreach (var prop in props)
					{
						console.WriteLine(" " + prop.Name + " = " + prop.GetValue(null), Console.Channel.System);
					}
				}
				else if (arguments[0].ToUpperInvariant().StartsWith("STACK."))
				{
					var variable = arguments[0].ToUpperInvariant().Replace("STACK.", string.Empty);

					var value = string.Empty;

					foreach (var prop in props)
					{
						if (prop.Name.ToUpperInvariant() == variable.Trim())
						{
							value = prop.GetValue(null).ToString();
						}
					}

					console.WriteLine(value, Console.Channel.System);
				}
			}
		}
	}
}
