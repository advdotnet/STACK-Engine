using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using STACK.Graphics;

namespace STACK.Input
{
	/// <summary>
	/// Captures input events send by the OS to the application using XNA Input methods.
	/// </summary>
	public class UserInputProvider : InputProvider
	{
		private KeyboardState _keyboardState, _oldKeyboardState;
		private MouseState _mouseState, _oldMouseState;
		private int _scrollValue, _oldScrollValue;
		private long _timeStamp;
		private readonly InputQueue _queue = new InputQueue();

		public override KeyboardState KeyboardState => _keyboardState;

		public override MouseState MouseState => _mouseState;

		private bool IsInsideWindow(int x, int y)
		{
			return DisplaySettings.Viewport.Bounds.Contains(x, y);
		}

		public override void Dispatch(bool paused)
		{
			Update();

			while (_queue.Count > 0)
			{
				var @event = _queue.Dequeue();
				@event.Paused = paused;
				Notify(@event);
			}
		}

		private void Update()
		{
			if (_queue == null)
			{
				return;
			}

			_timeStamp += (long)(GameSpeed.TickDuration);

			_oldKeyboardState = _keyboardState;
			_oldMouseState = _mouseState;
			_oldScrollValue = _scrollValue;

			_keyboardState = Keyboard.GetState();
			_mouseState = Mouse.GetState();
			_scrollValue = _mouseState.ScrollWheelValue;

			var screenCoordinates = new Vector2(_mouseState.X, _mouseState.Y);
			var worldCoordinates = DisplaySettings.TransformPoint(_mouseState.X, _mouseState.Y);

			_mouseState = _mouseState.UpdatePosition((int)worldCoordinates.X, (int)worldCoordinates.Y);

			// mouse move
			if (IsInsideWindow((int)screenCoordinates.X, (int)screenCoordinates.Y))
			{
				if (_oldScrollValue != _scrollValue)
				{
					_queue.Enqueue(InputEvent.MouseScroll(_timeStamp, _scrollValue - _oldScrollValue));
				}

				if (_oldMouseState.X != _mouseState.X || _oldMouseState.Y != _mouseState.Y)
				{
					_queue.Enqueue(InputEvent.MouseMove(_timeStamp, _mouseState.X, _mouseState.Y));
				}

				// mouse click left
				if (_mouseState.LeftButton == ButtonState.Pressed && _oldMouseState.LeftButton == ButtonState.Released)
				{
					_queue.Enqueue(InputEvent.MouseClick(ButtonState.Pressed, _timeStamp, MouseButton.Left));
				}

				if (_mouseState.LeftButton == ButtonState.Released && _oldMouseState.LeftButton == ButtonState.Pressed)
				{
					_queue.Enqueue(InputEvent.MouseClick(ButtonState.Released, _timeStamp, MouseButton.Left));
				}

				// mouse click right
				if (_mouseState.RightButton == ButtonState.Pressed && _oldMouseState.RightButton == ButtonState.Released)
				{
					_queue.Enqueue(InputEvent.MouseClick(ButtonState.Pressed, _timeStamp, MouseButton.Right));
				}

				if (_mouseState.RightButton == ButtonState.Released && _oldMouseState.RightButton == ButtonState.Pressed)
				{
					_queue.Enqueue(InputEvent.MouseClick(ButtonState.Released, _timeStamp, MouseButton.Right));
				}
			}

			// key down
			foreach (var key in _keyboardState.GetPressedKeys())
			{
				if (!_oldKeyboardState.IsKeyDown(key))
				{
					_queue.Enqueue(InputEvent.KeyPress(KeyState.Down, _timeStamp, key));
				}
			}

			// key up
			foreach (var key in _oldKeyboardState.GetPressedKeys())
			{
				if (!_keyboardState.IsKeyDown(key))
				{
					_queue.Enqueue(InputEvent.KeyPress(KeyState.Up, _timeStamp, key));
				}
			}
		}
	}
}
