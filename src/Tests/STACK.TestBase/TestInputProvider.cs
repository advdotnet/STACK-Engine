using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using STACK.Input;
using System.Collections.Generic;

namespace STACK.TestBase
{
    /// <summary>
    /// Allows to add input events programatically.
    /// </summary>
    public class TestInputProvider : InputProvider
    {
		private readonly Queue<InputEvent> _eventsToAdd = new Queue<InputEvent>();
		private readonly List<Keys> _pressedKeys = new List<Keys>();
		private int _mouseX, _mouseY;
		private ButtonState _left, _right;

		public override KeyboardState KeyboardState => new KeyboardState(_pressedKeys.ToArray());

		public override MouseState MouseState => new MouseState(_mouseX, _mouseY, 0, _left, ButtonState.Released, _right, ButtonState.Released, ButtonState.Released);

		public override void Dispatch(bool paused = false)
        {
            while (_eventsToAdd.Count > 0)
            {
                var @event = _eventsToAdd.Dequeue();
                @event.Paused = paused;
                Notify(@event);
            }
        }

        public void KeyDown(Keys key)
        {
            _eventsToAdd.Enqueue(InputEvent.KeyPress(KeyState.Down, 0, key));
            if (!_pressedKeys.Contains(key))
            {
                _pressedKeys.Add(key);
            }
        }

        public void KeyUp(Keys key)
        {
            _eventsToAdd.Enqueue(InputEvent.KeyPress(KeyState.Up, 0, key));
            if (_pressedKeys.Contains(key))
            {
                _pressedKeys.Remove(key);
            }
        }

        public void MouseDown(MouseButton button = MouseButton.Left)
        {
            _eventsToAdd.Enqueue(InputEvent.MouseClick(ButtonState.Pressed, 0, button));
            if (MouseButton.Left == button)
            {
                _left = ButtonState.Pressed;
            }
            else
            {
                _right = ButtonState.Pressed;
            }
        }

        public void MouseUp(MouseButton button = MouseButton.Left)
        {
            _eventsToAdd.Enqueue(InputEvent.MouseClick(ButtonState.Released, 0, button));
            if (MouseButton.Left == button)
            {
                _left = ButtonState.Released;
            }
            else
            {
                _right = ButtonState.Released;
            }
        }

        public void MouseMove(int x, int y)
        {
            _eventsToAdd.Enqueue(InputEvent.MouseMove(0, x, y));
            _mouseX = x;
            _mouseY = y;
        }

        public void MouseMove(Point p)
        {
            MouseMove(p.X, p.Y);
        }

        public void MouseClick(MouseButton button = MouseButton.Left)
        {
            MouseDown(button);
            MouseUp(button);
        }

        public void MouseClick(int x, int y, MouseButton button = MouseButton.Left)
        {
            MouseMove(x, y);
            MouseClick(button);
        }
    }
}
