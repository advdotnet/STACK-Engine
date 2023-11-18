using Microsoft.Xna.Framework;
using STACK.Input;

namespace STACK
{
	public interface IInteractive
	{
		void HandleInputEvent(Vector2 mouse, InputEvent inputEvent);
	}
}
