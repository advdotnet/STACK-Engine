using System.Collections.Generic;

namespace STACK.Input
{
	/// <summary>
	/// Queue of InputEvents.
	/// </summary>
	public class InputQueue : Queue<InputEvent>
	{
		private readonly InputEventFileLogger _logger;

		public InputQueue(bool record = false) : base(5)
		{
			if (record)
			{
				_logger = new InputEventFileLogger("demo.dat");
			}
		}

		public new void Enqueue(InputEvent item)
		{
			_logger?.Log(item);

			base.Enqueue(item);
		}

		public void OnUnload()
		{
			_logger?.Flush();
		}
	}
}
