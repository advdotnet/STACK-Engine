using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace STACK.Input
{
    /// <summary>
    /// Structure to model any input events like mouse or keyboard input.
    /// </summary>
    [Serializable]
    public struct InputEvent
    {
        public InputEventType Type { get; private set; }
        public long Timestamp { get; private set; }
        public int Param { get; private set; }
        public bool Handled { get; set; }
        public bool Paused { get; set; }

        private InputEvent(InputEventType type, long timestamp, int param) : this()
        {
            Type = type;
            Timestamp = timestamp;
            Param = param;
            Handled = false;
            Paused = false;
        }

        public void Dispatch(Vector2 mouse, Action<Vector2> mouseMove = null, 
            Action<Vector2, MouseButton> mouseDown = null, 
            Action<Vector2, MouseButton> mouseUp = null, 
            Action<Keys> keyDown = null,
            Action<Keys> keyUp = null)
        {

            switch (Type)
            {
                case InputEventType.MouseMove:
                    if (mouseMove != null)
                    {
                        mouseMove(IntToVector2(Param));
                    }
                    break;

                case InputEventType.MouseDown:
                    if (mouseDown != null)
                    {
                        mouseDown(mouse, (MouseButton)Param);
                    }
                    break;

                case InputEventType.MouseUp:
                    if (mouseUp != null)
                    {
                        mouseUp(mouse, (MouseButton)Param);
                    }
                    break;

                case InputEventType.KeyDown:
                    if (keyDown != null)
                    {
                        keyDown((Keys)Param);
                    }
                    break;

                case InputEventType.KeyUp:
                    if (keyUp != null)
                    {
                        keyUp((Keys)Param);
                    }
                    break;
            } 
        }

        /// <summary>
        /// Packs two shorts in an integer type.
        /// </summary>
        static int ShortsToInt(short x, short y)
        {
            return ((int)x << 16) | (int)(ushort)y;
        }

        public bool IsKeyPress(Keys key)
        {
            return (Type == InputEventType.KeyUp && (Keys)Param == key);
        }

        /// <summary>
        /// Extracts two shorts from an int type into a vector.
        /// </summary>
        public static Vector2 IntToVector2(int value)
        {
            int y = (short)(value & ushort.MaxValue);
            int x = (short)(value >> 16);

            return new Vector2(x, y);
        }

        public static InputEvent MouseClick(ButtonState state, long timestamp, MouseButton button)
        {
            return new InputEvent(state == ButtonState.Pressed ? InputEventType.MouseDown : InputEventType.MouseUp, timestamp, (int)button);
        }

        public static InputEvent KeyPress(KeyState state, long timestamp, Keys key)
        {
            return new InputEvent(state == KeyState.Down ? InputEventType.KeyDown : InputEventType.KeyUp, timestamp, (int)key);
        }

        public static InputEvent MouseMove(long timestamp, int x, int y)
        {
            return new InputEvent(InputEventType.MouseMove, timestamp, ShortsToInt((short)x, (short)y));
        }
    }
}
