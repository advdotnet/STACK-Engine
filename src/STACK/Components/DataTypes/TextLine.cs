using Microsoft.Xna.Framework;
using System;

namespace STACK
{
    [Serializable]
    public struct TextLine
    {
        string _Text;
        Vector2 _Position, _Origin;
        Rectangle _Hitbox;
        Color _Color;

        public string Text { get { return _Text; } }
        public Vector2 Position { get { return _Position; } }
        public Vector2 Origin { get { return _Origin; } }
        public Rectangle Hitbox { get { return _Hitbox; } }
        public Color Color { get { return _Color; } }

        public TextLine(string text, Vector2 position, Vector2 origin, Rectangle hitbox, Color color)
        {
            _Text = text;
            _Position = position;
            _Origin = origin;
            _Hitbox = hitbox;
            _Color = color;
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
