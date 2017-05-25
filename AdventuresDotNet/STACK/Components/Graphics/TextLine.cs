using System;
using Microsoft.Xna.Framework;

namespace STACK
{     
    [Serializable]
    public struct TextLine
    {
        public string Text { get; }
        public Vector2 Position { get; }
        public Vector2 Origin { get; }
        public Rectangle Hitbox { get; }
        public Color Color { get; }

        public TextLine(string text, Vector2 position, Vector2 origin, Rectangle hitbox, Color color)
        {
            Text = text;
            Position = position;
            Origin = origin;
            Hitbox = hitbox;
            Color = color;
        }

        public TextLine ChangeColor(Color value)
        {
            return new TextLine(Text, Position, Origin, Hitbox, value);
        }

        public TextLine Move(Vector2 value)
        {
            var position = Position - value;
            var hitbox = new Rectangle(Hitbox.X - (int)value.X, Hitbox.Y - (int)value.Y, Hitbox.Width, Hitbox.Height);

            return new TextLine(Text, position, Origin, hitbox, Color);
        }

        public TextLine ChangeHitbox(Rectangle rectangle)
        {
            return new TextLine(Text, Position, Origin, rectangle, Color);
        }
    }
}
