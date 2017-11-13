using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;

namespace STACK
{
    public static class ExtensionMethods
    {
        public static Vector3 ToInt(this Vector3 value)
        {
            return new Vector3((int)value.X, (int)value.Y, (int)value.Z);
        }

        public static Vector2 ToInt(this Vector2 value)
        {
            return new Vector2((int)value.X, (int)value.Y);
        }

        public static Vector2 ToVector2(this Point value)
        {
            return new Vector2(value.X, value.Y);
        }

        public static Point ToPoint(this Vector2 value)
        {
            return new Point((int)value.X, (int)value.Y);
        }

        /// <summary>
        /// Required for FNA.
        /// </summary>
        public static bool Contains(this Rectangle rect, Vector2 point)
        {
            return rect.Contains(new Point((int)point.X, (int)point.Y));
        }

        public static MouseState UpdatePosition(this MouseState state, int x, int y)
        {
            return new MouseState(x, y, state.ScrollWheelValue, state.LeftButton,
                state.MiddleButton, state.RightButton, state.XButton1, state.XButton2);
        }

        public static bool IsPlaying(this SoundEffectInstance sound)
        {
            if (null == sound)
            {
                return false;
            }

            return (SoundState.Playing == sound.State);
        }
    }
}
