using Microsoft.Xna.Framework;
using STACK.Input;
using System;

namespace STACK.Components
{
	/// <summary>
	/// Component holding the mouse position as well as the object which is 
	/// currently under the mouse.
	/// </summary>
	[Serializable]
	public class Mouse : Component, IUpdate, IInteractive
	{
		private Vector2 _position;
		private Entity _objectUnderMouse;
		private bool _enabled;
		private float _updateOrder;

		public Vector2 Position { get => _position; set => _position = value; }
		public Entity ObjectUnderMouse => _objectUnderMouse;
		public bool Enabled { get => _enabled; set => _enabled = value; }
		public float UpdateOrder { get => _updateOrder; set => _updateOrder = value; }

		public Mouse()
		{
			Enabled = true;
		}

		public static Mouse Create(World addTo)
		{
			return addTo.Add<Mouse>();
		}

		public void HandleInputEvent(Vector2 mouse, InputEvent inputEvent)
		{
			if (inputEvent.Type == InputEventType.MouseMove)
			{
				Position = InputEvent.IntToVector2(inputEvent.Param);

				_objectUnderMouse = World.GetObjectAtPosition(Position);
			}
		}

		public void Update()
		{
			_objectUnderMouse = World.GetObjectAtPosition(Position);
		}

		public Mouse SetPosition(Vector2 value) { Position = value; return this; }
		public Mouse SetPosition(float x, float y) { Position = new Vector2(x, y); return this; }
	}
}
