using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using STACK.Components;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace STACK
{
    [Serializable]
    public class Text : Component, IContent, IDraw, IUpdate, IInitialize
    {
        Alignment _Align = Alignment.Center;
        RenderStage _RenderStage = RenderStage.PostBloom;

        [NonSerialized]
        public Func<string, Vector2> MeasureStringFn = null;
        public Alignment Align { get { return _Align; } set { _Align = value; } }
        public SpriteFont SpriteFont { get { return _SpriteFont; } }
        public Rectangle Bounds;
        public Vector2 Offset = Vector2.Zero;
        public RenderStage RenderStage { get { return _RenderStage; } set { _RenderStage = value; } }
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
        SpriteFont _SpriteFont;
        Vector2 Position;
        Color _Color = Color.White;
        float Timer = 0;
        float AlphaPercentage = 1;
        float Duration = -1;

        Vector2 _ConstrainOffset;
        bool _Visible;
        float _DrawOrder;
        bool _Enabled;
        float _UpdateOrder;

        public Vector2 ConstrainOffset { get { return _ConstrainOffset; } }
        public bool Visible { get { return _Visible; } set { _Visible = value; } }
        public float DrawOrder { get { return _DrawOrder; } set { _DrawOrder = value; } }
        public bool Enabled { get { return _Enabled; } set { _Enabled = value; } }
        public float UpdateOrder { get { return _UpdateOrder; } set { _UpdateOrder = value; } }

        Vector2 TextBounds = Vector2.Zero;

        public Color Color
        {
            get
            {
                return _Color;
            }
            set
            {
                _Color = value;
                for (int i = 0; i < Lines.Count; i++)
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
        void OnDeserialized(StreamingContext c)
        {
            OnSerialized(c);
        }

        public void Initialize(bool restore)
        {
            Transform = Get<Transform>();
        }

        public void LoadContent(ContentLoader content)
        {
            _SpriteFont = content.Load<SpriteFont>(Font);

            // use SpriteFont's MeasureString method as default - can be overriden in tests
            if (null == MeasureStringFn)
            {
                MeasureStringFn = _SpriteFont.MeasureString;
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

            Duration = TextDuration.Default(text, duration);

            if (Duration >= 0)
            {
                Duration += 2 * FadeDuration;
                AlphaPercentage = 0;
            }

            CreateLines(position, wrappedText);
            _ConstrainOffset = ConstrainBounds();
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
                var ConstrainedBounds = ConstrainRectangle(Bounds, ConstrainingRectangle);
                var Diff = ConstrainedBounds.Center - (Position).ToPoint();
                return new Vector2(Diff.X, 0);
            }

            return Vector2.Zero;
        }

        public static Rectangle ConstrainRectangle(Rectangle inner, Rectangle outer)
        {
            if (outer.Contains(inner))
            {
                return inner;
            }

            int Width = Math.Min(inner.Width, outer.Width);
            int Height = Math.Min(inner.Height, outer.Height);
            int X = inner.X;
            int Y = inner.Y;

            if (X < outer.X) X = outer.X;
            if (Y < outer.Y) Y = outer.Y;

            if (X + Width > outer.Right) X = outer.Right - Width;
            if (Y + Height > outer.Bottom) Y = outer.Bottom - Height;

            return new Rectangle(X, Y, Width, Height);
        }

        /// <summary>
        /// Takes a string and performs word wrapping by adding newline 
        /// characters when needed.
        /// </summary>
        private string WordWrapText(string text)
        {
            const char SPACE = ' ';
            if (string.IsNullOrEmpty(text) || !WordWrap || MeasureStringFn == null)
            {
                return text;
            }

            var Line = string.Empty;
            var Result = string.Empty;
            var Words = text.Split(SPACE);

            foreach (string Word in Words)
            {
                if (MeasureStringFn(Line + Word).X > Bounds.Width)
                {
                    Result = Result + Line + '\n';
                    Line = string.Empty;
                }

                Line = Line + Word + SPACE;
            }

            return Result + Line.TrimEnd(SPACE);
        }

        void CreateLines(Vector2 position, string wrappedText)
        {
            TextBounds = MeasureStringFn(wrappedText);
            var TextPosition = position;
            var Origin = TextBounds * 0.5f;

            if (Align.Has(Alignment.Top))
            {
                Origin.Y += Bounds.Height / 2f - TextBounds.Y / 2f;
            }

            if (Align.Has(Alignment.Bottom))
            {
                Origin.Y -= Bounds.Height / 2f - TextBounds.Y / 2f;
            }

            string[] TextLines = wrappedText.Split('\n');

            Lines.Clear();

            for (int i = 0; i < TextLines.Length; i++)
            {
                var LineSize = MeasureStringFn(TextLines[i]);
                var LineOffset = new Vector2((TextBounds.X - LineSize.X) * 0.5f, i * (TextBounds.Y / TextLines.Length + 3));

                if (Align.Has(Alignment.Left))
                {
                    LineOffset.X -= Bounds.Width / 2f - LineSize.X / 2f;
                }

                if (Align.Has(Alignment.Right))
                {
                    LineOffset.X += Bounds.Width / 2f - LineSize.X / 2f;
                }

                var LinePosition = TextPosition + LineOffset;
                var Hitbox = new Rectangle((int)(LinePosition.X - Origin.X), (int)(LinePosition.Y - Origin.Y), (int)LineSize.X, (int)LineSize.Y);
                var NewLine = new TextLine(TextLines[i], LinePosition, Origin, Hitbox, Color);

                Lines.Add(NewLine);
            }

            Position = position;
        }

        public void Clear()
        {
            Lines.Clear();
            Position = Vector2.Zero;
            Timer = 0;
            Duration = -1;
            AlphaPercentage = 1;
        }

        public void SetPosition(Vector2 position)
        {
            if (Position != position)
            {
                var delta = Position - position;
                for (int i = 0; i < Lines.Count; i++)
                {
                    Lines[i] = Lines[i].Move(delta);
                }

                Position = position;
                SetBounds(position);
                _ConstrainOffset = ConstrainBounds();
            }
        }

        public void Update()
        {
            if (Duration > 0)
            {
                if (Timer >= Duration)
                {
                    Clear();
                }

                AlphaPercentage = 1.0f;

                if (Timer <= FadeDuration)
                {
                    AlphaPercentage = 1 - (FadeDuration - Timer) / FadeDuration;
                }
                else if (Timer >= Duration - FadeDuration)
                {
                    AlphaPercentage = ((Duration - Timer)) / FadeDuration;
                }

                Timer += GameSpeed.TickDuration;
            }
        }

        public void Draw(Graphics.Renderer renderer)
        {
            if (RenderStage != renderer.Stage || Lines.Count == 0 || _SpriteFont == null)
            {
                return;
            }

            var TransformIsAbsolute = (null != Transform && Transform.Absolute);
            var Camera = Entity.DrawScene.Get<Camera>();

            for (int i = 0; i < Lines.Count; i++)
            {
                var Line = Lines[i];
                var CurrentColor = new Color(Line.Color.R, Line.Color.G, Line.Color.B, (byte)(Line.Color.A * AlphaPercentage));
                var LinePositon = (Line.Position - Offset).ToInt();

                if (TransformIsAbsolute)
                {
                    LinePositon = Camera.TransformInverse(LinePositon);
                }

                LinePositon += ConstrainOffset;
                var LineOrigin = Line.Origin.ToInt();

                renderer.SpriteBatch.DrawString(_SpriteFont, Line.Text, LinePositon, CurrentColor, 0, LineOrigin, 1, SpriteEffects.None, 0);
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
