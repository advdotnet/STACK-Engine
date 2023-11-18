using System;

namespace STACK.Debug
{
	/// <summary>
	/// Sets a variable.
	/// </summary>
	internal class SetCommand : IConsoleCommand
	{
		public string Name => "set";

		public string Description => "Sets a variable.";

#pragma warning disable IDE0052 // Ungelesene private Member entfernen
		private readonly StackEngine _engine;
#pragma warning restore IDE0052 // Ungelesene private Member entfernen

		public SetCommand(StackEngine game)
		{
			_engine = game;
		}

		public static T Parse<T>(string value)
		{
			return (T)System.ComponentModel.TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(value);
		}

		public void Execute(Console console, string[] arguments)
		{
			if (arguments.Length == 3 && arguments[1] == "=" && arguments[0].Contains("."))
			{
				var variableName = arguments[0].Split('.')[1].ToUpperInvariant();
				var properties = typeof(EngineVariables).GetFields();
				var value = arguments[2].Trim();

				foreach (var prop in properties)
				{
					if (prop.Name.ToUpperInvariant() == variableName)
					{
						try
						{
							var test = prop.FieldType;
							var method = typeof(SetCommand).GetMethod("Parse").MakeGenericMethod(new Type[] { test });
							var result = method.Invoke(this, new object[] { value });

							prop.SetValue(null, result);
							value = prop.GetValue(null).ToString();

							console.WriteLine(value, Console.Channel.System);
						}
						catch
						{
							console.WriteLine("Could not set value.", Console.Channel.Error);
						}

						return;
					}
				}

				console.WriteLine("Variable not found.", Console.Channel.Error);
			}
			else
			{
				console.WriteLine("Syntax is SET <namespace>.<variable> = <value>", Console.Channel.Error);
			}
		}
	}
}
