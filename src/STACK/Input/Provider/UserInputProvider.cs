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
        KeyboardState _KeyboardState, OldKeyboardState;
        MouseState _MouseState, OldMouseState;
        int _ScrollValue, OldScrollValue;
        long TimeStamp;
        InputQueue Queue = new InputQueue();

        public override KeyboardState KeyboardState
        {
            get
            {
                return _KeyboardState;
            }
        }

        public override MouseState MouseState
        {
            get
            {
                return _MouseState;
            }
        }

        bool IsInsideWindow(int x, int y)
        {
            return DisplaySettings.Viewport.Bounds.Contains(x, y);
        }

        public override void Dispatch(bool paused)
        {
            Update();

            while (Queue.Count > 0)
            {
                var Event = Queue.Dequeue();
                Event.Paused = paused;
                Notify(Event);
            }
        }

        void Update()
        {
            if (Queue == null)
            {
                return;
            }

            TimeStamp += (long)(GameSpeed.TickDuration);

            OldKeyboardState = _KeyboardState;
            OldMouseState = _MouseState;
            OldScrollValue = _ScrollValue;

            _KeyboardState = Keyboard.GetState();
            _MouseState = Mouse.GetState();
            _ScrollValue = _MouseState.ScrollWheelValue;

            var ScreenCoordinates = new Vector2(_MouseState.X, _MouseState.Y);
            var WorldCoordinates = DisplaySettings.TransformPoint(_MouseState.X, _MouseState.Y);

            _MouseState = _MouseState.UpdatePosition((int)WorldCoordinates.X, (int)WorldCoordinates.Y);

            // mouse move
            if (IsInsideWindow((int)ScreenCoordinates.X, (int)ScreenCoordinates.Y))
            {
                if (OldScrollValue != _ScrollValue)
                {
                    Queue.Enqueue(InputEvent.MouseScroll(TimeStamp, _ScrollValue - OldScrollValue));
                }

                if (OldMouseState.X != _MouseState.X || OldMouseState.Y != _MouseState.Y)
                {
                    Queue.Enqueue(InputEvent.MouseMove(TimeStamp, _MouseState.X, _MouseState.Y));
                }

                // mouse click left
                if (_MouseState.LeftButton == ButtonState.Pressed && OldMouseState.LeftButton == ButtonState.Released)
                {
                    Queue.Enqueue(InputEvent.MouseClick(ButtonState.Pressed, TimeStamp, MouseButton.Left));
                }

                if (_MouseState.LeftButton == ButtonState.Released && OldMouseState.LeftButton == ButtonState.Pressed)
                {
                    Queue.Enqueue(InputEvent.MouseClick(ButtonState.Released, TimeStamp, MouseButton.Left));
                }

                // mouse click right
                if (_MouseState.RightButton == ButtonState.Pressed && OldMouseState.RightButton == ButtonState.Released)
                {
                    Queue.Enqueue(InputEvent.MouseClick(ButtonState.Pressed, TimeStamp, MouseButton.Right));
                }

                if (_MouseState.RightButton == ButtonState.Released && OldMouseState.RightButton == ButtonState.Pressed)
                {
                    Queue.Enqueue(InputEvent.MouseClick(ButtonState.Released, TimeStamp, MouseButton.Right));
                }
            }

            // key down
            foreach (var Key in _KeyboardState.GetPressedKeys())
            {
                if (!OldKeyboardState.IsKeyDown(Key))
                {
                    Queue.Enqueue(InputEvent.KeyPress(KeyState.Down, TimeStamp, Key));
                }
            }

            // key up
            foreach (var Key in OldKeyboardState.GetPressedKeys())
            {
                if (!_KeyboardState.IsKeyDown(Key))
                {
                    Queue.Enqueue(InputEvent.KeyPress(KeyState.Up, TimeStamp, Key));
                }
            }
        }
    }
}
