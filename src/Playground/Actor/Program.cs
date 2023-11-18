using STACK;
using System;
using System.IO;

namespace Actor
{

	/// <summary>
	/// The main class.
	/// </summary>
	public static class Program
	{

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			AppDomain.CurrentDomain.UnhandledException += (sender, e)
				=> FatalExceptionObject(e.ExceptionObject);

			using (var game = new Window(new ActorGame()))
			{
				game.IsMouseVisible = false;
				game.Run();
			}
		}

		static void FatalExceptionObject(object exceptionObject)
		{
			var exception = exceptionObject as Exception ?? new NotSupportedException(
				  "Unhandled exception doesn't derive from System.Exception: "
				   + exceptionObject.ToString()
				);
			HandleException(exception);
		}

		static void HandleException(Exception e)
		{
			AppendToFile("error.log", e.ToString());
		}

		static void AppendToFile(string filename, string text)
		{
			using (var w = File.AppendText(filename))
			{
				w.WriteLine(text);
			}
		}
	}
}
