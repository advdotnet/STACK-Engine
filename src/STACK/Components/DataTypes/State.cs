using System;

namespace STACK.Components
{
	[Flags]
	public enum State
	{
		Idle = 0,
		Walking = 1,
		Talking = 2,
		Custom = 4
	}
}
