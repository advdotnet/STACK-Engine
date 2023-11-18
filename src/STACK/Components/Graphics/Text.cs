using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using STACK.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace STACK
{
	[Serializable]
	public class Text : Component, IContent, IDraw, IUpdate, IInitialize
	{
		private Alignment _align = Alignment.Center;
		private RenderStage _renderStage = RenderStage.PostBloom;

		[NonSerialized]
		public Func<string, Vector2> MeasureStringFn = null;
		public Alignment Align { get => _align; set => _align = value; }
		public SpriteFont SpriteFont => _spriteFont;
		public Rectangle Bounds;
		public Vector2 Offset = Vector2.Zero;
		public RenderStage RenderStage { get => _renderStage; set => _renderStage = value; }
		public List<TextLine> Lines = new List<TextLine>(5);
		public Rectangle ConstrainingRectangle = Rectangle.Empty;
		public bool Constrain = false;

		/// <summary>
		/// Fadein and fadeout duration in seconds.
		/// </summary>
		public float FadeDuration = 0.0f;
		public bool WordWrap = true;
		public int Width = 400;
		public int Height = 100;
		public string Font = "fonts/stack";

		[NonSerialized]
		internal Transform Transform = null;

		[NonSerialized]
		private SpriteFont _spriteFont;
		private Vector2 _position;
		private Color _color = Color.White;
		private float _timer = 0;
		private float _alphaPercentage = 1;
		private float _duration = -1;
		private Vector2 _constrainOffset;
		private bool _visible;
		private float _drawOrder;
		private bool _enabled;
		private float _updateOrder;
		private const char _space = ' ';
		private const char _nl = '\n';
		private Vector2 _textBounds = Vector2.Zero;

		public Vector2 ConstrainOffset => _constrainOffset;
		public bool Visible { get => _visible; set => _visible = value; }
		public float DrawOrder { get => _drawOrder; set => _drawOrder = value; }
		public bool Enabled { get => _enabled; set => _enabled = value; }
		public float UpdateOrder { get => _updateOrder; set => _updateOrder = value; }

		public Color Color
		{
			get => _color;
			set
			{
				_color = value;
				for (var i = 0; i < Lines.Count; i++)
				{
					Lines[i] = Lines[i].ChangeColor(value);
				}
			}
		}

		public Text()
		{
			Enabled = true;
			Visible = true;
		}

		[OnSerializing]
		public void OnSerializing(StreamingContext context)
		{
			if (null != Lines && Lines.Count == 0)
			{
				Lines = null;
			}
		}

		[OnSerialized]
		public void OnSerialized(StreamingContext context)
		{
			if (null == Lines)
			{
				Lines = new List<TextLine>(5);
			}
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext c)
		{
			OnSerialized(c);
		}

		public void Initialize(bool restore)
		{
			Transform = Get<Transform>();
		}

		public void LoadContent(ContentLoader content)
		{
			_spriteFont = content.Load<SpriteFont>(Font);

			// use SpriteFont's MeasureString method as default - can be overriden in tests
			if (null == MeasureStringFn)
			{
				MeasureStringFn = _spriteFont.MeasureString;
			}
		}

		public void UnloadContent() { }

		public void Set(string text, float duration, Vector2 position)
		{
			if (null == text)
			{
				text = string.Empty;
			}

			SetBounds(position);

			var wrappedText = WordWrapText(text);

			_duration = TextDuration.Default(text, duration);

			if (_duration >= 0)
			{
				_duration += 2 * FadeDuration;
				_alphaPercentage = 0;
			}

			CreateLines(position, wrappedText, null);
			_constrainOffset = ConstrainBounds();
		}

		public void Set(List<TextInfo> textInfos, float duration, Vector2 position)
		{
			if (null == textInfos)
			{
				return;
			}

			SetBounds(position);
			var tags = new List<string>();
			var wrappedText = string.Empty;

			foreach (var textInfo in textInfos)
			{
				var currentWrappedText = WordWrapText(textInfo.Text);
				var linesCount = currentWrappedText.Count(x => x == _nl) + 1;
				tags.AddRange(Enumerable.Repeat(textInfo.Tag, linesCount));
				wrappedText += currentWrappedText + _nl;
			}

			wrappedText = wrappedText.TrimEnd(_nl);

			_duration = TextDuration.Default(wrappedText, duration);

			if (_duration >= 0)
			{
				_duration += 2 * FadeDuration;
				_alphaPercentage = 0;
			}

			CreateLines(position, wrappedText, tags);
			_constrainOffset = ConstrainBounds();
		}

		private void SetBounds(Vector2 position)
		{
			Bounds.X = (int)position.X - Width / 2;
			Bounds.Y = (int)position.Y - Height / 2;
			Bounds.Width = Width;
			Bounds.Height = Height;
		}

		private Vector2 ConstrainBounds()
		{
			if (Constrain && !ConstrainingRectangle.Contains(Bounds))
			{
				var constrainedBounds = ConstrainRectangle(Bounds, ConstrainingRectangle);
				var diff = constrainedBounds.Center - _position.ToPoint();
				return new Vector2(diff.X, 0);
			}

			return Vector2.Zero;
		}

		public static Rectangle ConstrainRectangle(Rectangle inner, Rectangle outer)
		{
			if (outer.Contains(inner))
			{
				return inner;
			}

			var width = Math.Min(inner.Width, outer.Width);
			var height = Math.Min(inner.Height, outer.Height);
			var x = inner.X;
			var y = inner.Y;

			if (x < outer.X)
			{
				x = outer.X;
			}

			if (y < outer.Y)
			{
				y = outer.Y;
			}

			if (x + width > outer.Right)
			{
				x = outer.Right - width;
			}

			if (y + height > outer.Bottom)
			{
				y = outer.Bottom - height;
			}

			return new Rectangle(x, y, width, height);
		}

		/// <summary>
		/// Takes a string and performs word wrapping by adding newline 
		/// characters when needed.
		/// </summary>
		private string WordWrapText(string text)
		{

			if (string.IsNullOrEmpty(text) || !WordWrap || MeasureStringFn == null)
			{
				return text;
			}

			var line = string.Empty;
			var result = string.Empty;
			var words = text.Split(_space);

			foreach (var word in words)
			{
				if (MeasureStringFn(line + word).X > Bounds.Width)
				{
					result = result + line + _nl;
					line = string.Empty;
				}

				line = line + word + _space;
			}

			return result + line.TrimEnd(_space);
		}

		private void CreateLines(Vector2 position, string wrappedText, List<string> tags = null)
		{
			_textBounds = MeasureStringFn(wrappedText);
			var textPosition = position;
			var origin = _textBounds * 0.5f;

			if (Align.Has(Alignment.Top))
			{
				origin.Y += Bounds.Height / 2f - _textBounds.Y / 2f;
			}

			if (Align.Has(Alignment.Bottom))
			{
				origin.Y -= Bounds.Height / 2f - _textBounds.Y / 2f;
			}

			var textLines = wrappedText.Split(_nl);

			Lines.Clear();

			for (var i = 0; i < textLines.Length; i++)
			{
				var lineSize = MeasureStringFn(textLines[i]);
				var lineOffset = new Vector2((_textBounds.X - lineSize.X) * 0.5f, i * (_textBounds.Y / textLines.Length + 3));

				if (Align.Has(Alignment.Left))
				{
					lineOffset.X -= Bounds.Width / 2f - lineSize.X / 2f;
				}

				if (Align.Has(Alignment.Right))
				{
					lineOffset.X += Bounds.Width / 2f - lineSize.X / 2f;
				}

				var linePosition = textPosition + lineOffset;
				var hitbox = new Rectangle((int)(linePosition.X - origin.X), (int)(linePosition.Y - origin.Y), (int)lineSize.X, (int)lineSize.Y);
				string tag = null;
				if (tags != null && i < tags.Count)
				{
					tag = tags[i];
				}

				var newLine = new TextLine(textLines[i], linePosition, origin, hitbox, Color, tag);

				Lines.Add(newLine);
			}

			_position = position;
		}

		public void Clear()
		{
			Lines.Clear();
			_position = Vector2.Zero;
			_timer = 0;
			_duration = -1;
			_alphaPercentage = 1;
		}

		public void SetPosition(Vector2 position)
		{
			if (_position != position)
			{
				var delta = _position - position;
				for (var i = 0; i < Lines.Count; i++)
				{
					Lines[i] = Lines[i].Move(delta);
				}

				_position = position;
				SetBounds(position);
				_constrainOffset = ConstrainBounds();
			}
		}

		public void Update()
		{
			if (_duration > 0)
			{
				if (_timer >= _duration)
				{
					Clear();
				}

				_alphaPercentage = 1.0f;

				if (_timer <= FadeDuration)
				{
					_alphaPercentage = 1 - (FadeDuration - _timer) / FadeDuration;
				}
				else if (_timer >= _duration - FadeDuration)
				{
					_alphaPercentage = ((_duration - _timer)) / FadeDuration;
				}

				_timer += GameSpeed.TickDuration;
			}
		}

		public void Draw(Graphics.Renderer renderer)
		{
			if (RenderStage != renderer.Stage || Lines.Count == 0 || _spriteFont == null)
			{
				return;
			}

			var transformIsAbsolute = (null != Transform && Transform.Absolute);
			var camera = Entity.DrawScene.Get<Camera>();

			for (var i = 0; i < Lines.Count; i++)
			{
				var line = Lines[i];
				var currentColor = new Color(line.Color.R, line.Color.G, line.Color.B, (byte)(line.Color.A * _alphaPercentage));
				var linePositon = (line.Position - Offset).ToInt();

				if (transformIsAbsolute)
				{
					linePositon = camera.TransformInverse(linePositon);
				}

				linePositon += ConstrainOffset;
				var lineOrigin = (line.Origin).ToInt();
				if (lineOrigin.X > linePositon.X)
				{
					lineOrigin.X = (int)linePositon.X;
				}

				renderer.SpriteBatch.DrawString(_spriteFont, line.Text, linePositon, currentColor, 0, lineOrigin, 1, SpriteEffects.None, 0);
			}
		}

		public static Text Create(Entity addTo)
		{
			return addTo.Add<Text>();
		}

		public Text SetFont(string value) { Font = value; return this; }
		public Text SetHeight(int value) { Height = value; return this; }
		public Text SetWordWrap(bool value) { WordWrap = value; return this; }
		public Text SetAlign(Alignment value) { Align = value; return this; }
		public Text SetColor(Color value) { Color = value; return this; }
		public Text SetWidth(int value) { Width = value; return this; }
		public Text SetRenderStage(RenderStage value) { RenderStage = value; return this; }
		public Text SetConstrain(bool value) { Constrain = value; return this; }
		public Text SetConstrainingRectangle(Rectangle value) { ConstrainingRectangle = value; return this; }
		public Text SetVisible(bool value) { Visible = value; return this; }
		public Text SetFadeDuration(float value) { FadeDuration = value; return this; }
	}
}
