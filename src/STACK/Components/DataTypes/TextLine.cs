using Microsoft.Xna.Framework;
using System;

namespace STACK
{
	[Serializable]
	public struct TextInfo
	{
		public string Text;
		public string Tag;

		public TextInfo(string text, string tag = null)
		{
			Text = text;
			Tag = tag;
		}
	}

	[Serializable]
	public struct TextLine
	{
		private readonly string _text, _tag;
		private Vector2 _position, _origin;
		private Rectangle _hitbox;
		private Color _color;

		public string Text => _text;
		public Vector2 Position => _position;
		public Vector2 Origin => _origin;
		public Rectangle Hitbox => _hitbox;
		public Color Color => _color;
		public string Tag => _tag;

		public TextLine(string text, Vector2 position, Vector2 origin, Rectangle hitbox, Color color, string tag = null)
		{
			_text = text;
			_position = position;
			_origin = origin;
			_hitbox = hitbox;
			_color = color;
			_tag = tag;
		}

		public TextLine ChangeColor(Color value)
		{
			return new TextLine(Text, Position, Origin, Hitbox, value, Tag);
		}

		public TextLine Move(Vector2 value)
		{
			var position = Position - value;
			var hitbox = new Rectangle(Hitbox.X - (int)value.X, Hitbox.Y - (int)value.Y, Hitbox.Width, Hitbox.Height);

			return new TextLine(Text, position, Origin, hitbox, Color, Tag);
		}

		public TextLine ChangeHitbox(Rectangle rectangle)
		{
			return new TextLine(Text, Position, Origin, rectangle, Color, Tag);
		}
	}
}
