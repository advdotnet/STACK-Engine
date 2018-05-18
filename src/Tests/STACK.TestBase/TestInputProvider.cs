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
        Queue<InputEvent> EventsToAdd = new Queue<InputEvent>();
        List<Keys> PressedKeys = new List<Keys>();
        int MouseX, MouseY;
        ButtonState Left, Right;

        public override KeyboardState KeyboardState
        {
            get
            {
                return new KeyboardState(PressedKeys.ToArray());
            }
        }

        public override MouseState MouseState
        {
            get
            {
                return new MouseState(MouseX, MouseY, 0, Left, ButtonState.Released, Right, ButtonState.Released, ButtonState.Released);
            }
        }

        public override void Dispatch(bool paused = false)
        {
            while (EventsToAdd.Count > 0)
            {
                var Event = EventsToAdd.Dequeue();
                Event.Paused = paused;
                Notify(Event);
            }
        }

        public void KeyDown(Keys key)
        {
            EventsToAdd.Enqueue(InputEvent.KeyPress(KeyState.Down, 0, key));
            if (!PressedKeys.Contains(key))
            {
                PressedKeys.Add(key);
            }
        }

        public void KeyUp(Keys key)
        {
            EventsToAdd.Enqueue(InputEvent.KeyPress(KeyState.Up, 0, key));
            if (PressedKeys.Contains(key))
            {
                PressedKeys.Remove(key);
            }
        }

        public void MouseDown(MouseButton button = MouseButton.Left)
        {
            EventsToAdd.Enqueue(InputEvent.MouseClick(ButtonState.Pressed, 0, button));
            if (MouseButton.Left == button)
            {
                Left = ButtonState.Pressed;
            }
            else
            {
                Right = ButtonState.Pressed;
            }
        }

        public void MouseUp(MouseButton button = MouseButton.Left)
        {
            EventsToAdd.Enqueue(InputEvent.MouseClick(ButtonState.Released, 0, button));
            if (MouseButton.Left == button)
            {
                Left = ButtonState.Released;
            }
            else
            {
                Right = ButtonState.Released;
            }
        }

        public void MouseMove(int x, int y)
        {
            EventsToAdd.Enqueue(InputEvent.MouseMove(0, x, y));
            MouseX = x;
            MouseY = y;
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
