using Microsoft.Xna.Framework.Input;
using STACK.Graphics;

namespace STACK.Input
{
	/// <summary>   
	/// Abstract base class for input providers
	/// </summary>
	public abstract class InputProvider
	{
		public delegate void InputHandler(InputEvent check);
		public event InputHandler Handler;
		public DisplaySettings DisplaySettings { get; set; }

		protected void Notify(InputEvent @event)
		{
			// avoids racing conditions
			Handler?.Invoke(@event);
		}

		public abstract void Dispatch(bool paused);
		public abstract KeyboardState KeyboardState { get; }
		public abstract MouseState MouseState { get; }
	}
}
