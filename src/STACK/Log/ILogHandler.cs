namespace STACK.Logging
{
	public interface ILogHandler
	{
		void WriteLine(string text, LogLevel level);
	}
}
