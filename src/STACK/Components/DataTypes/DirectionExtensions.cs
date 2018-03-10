using Microsoft.Xna.Framework;
using System;

namespace STACK.Components
{
    public static class Direction
    {
        public static Vector2 ToVector2(this Directions4 direction)
        {
            switch (direction)
            {
                case Directions4.Up: return new Vector2(0, -1);
                case Directions4.Right: return new Vector2(1, 0);
                case Directions4.Down: return new Vector2(0, 1);
                case Directions4.Left: return new Vector2(-1, 0);
            }
            return Vector2.Zero;
        }

        public static Vector2 ToVector2(this Directions8 direction)
        {
            switch (direction)
            {
                case Directions8.LeftUp: return new Vector2(-1, -1);
                case Directions8.Up: return new Vector2(0, -1);
                case Directions8.RightUp: return new Vector2(1, -1);
                case Directions8.Right: return new Vector2(1, 0);
                case Directions8.RightDown: return new Vector2(1, 1);
                case Directions8.Down: return new Vector2(0, 1);
                case Directions8.LeftDown: return new Vector2(-1, 1);
                case Directions8.Left: return new Vector2(-1, 0);
            }
            return Vector2.Zero;
        }

        public static Directions4 ToDirection4(this Vector2 value)
        {
            if (value.Equals(Vector2.Zero))
            {
                return Directions4.None;
            }

            if (Math.Abs(value.X) > Math.Abs(value.Y))
            {
                if (value.X > 0)
                {
                    return Directions4.Right;
                }
                else
                {
                    return Directions4.Left;
                }
            }
            else
            {
                if (value.Y > 0)
                {
                    return Directions4.Down;
                }
                else
                {
                    return Directions4.Up;
                }
            }
        }

        public static Directions8 ToDirection8(this Vector2 value)
        {
            var AbsX = Math.Abs(value.X);
            var AbsY = Math.Abs(value.Y);

            if (AbsX == 0 && AbsY == 0)
            {
                return Directions8.None;
            }

            if (AbsX > AbsY)
            {
                // vertical side
                double Half = AbsX * 0.4142;
                if (value.X > 0)
                {
                    // left side
                    if (value.Y > Half) return Directions8.LeftDown;
                    if (value.Y < -Half) return Directions8.LeftUp;
                    return Directions8.Left;
                }
                else
                {
                    // right side
                    if (value.Y > Half) return Directions8.RightDown;
                    if (value.Y < -Half) return Directions8.RightUp;
                    return Directions8.Right;
                }
            }
            else
            {
                // horizontal side
                var Half = AbsY * 0.4142;
                if (value.Y > 0)
                {
                    // bottom
                    if (value.X > Half) return Directions8.RightDown;
                    if (value.X < -Half) return Directions8.LeftDown;
                    return Directions8.Down;
                }
                else
                {
                    // top
                    if (value.X > Half) return Directions8.RightUp;
                    if (value.X < -Half) return Directions8.LeftUp;
                    return Directions8.Up;
                }
            }
        }
    }
}
