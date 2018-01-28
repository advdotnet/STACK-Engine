using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using STACK.Graphics;
using STACK.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace STACK.Components
{
    /// <summary>
    /// Represents a sequence of frames used for animations.
    /// </summary>
    [Serializable]
    public class Frames : List<int>
    {
        public Frames(IEnumerable<int> collection) : base(collection)
        {
        }

        /// <summary>
        /// Adds the given value to all frame numbers.
        /// </summary>
        /// <param name="value">value to add</param>
        /// <returns></returns>
        public Frames Shift(int value)
        {
            for (int i = 0; i < this.Count; i++)
            {
                this[i] += value;
            }

            return this;
        }

        /// <summary>
        /// Reverses the order of the elements in the entire list.
        /// </summary>
        /// <returns></returns>
        public new Frames Reverse()
        {
            base.Reverse();

            return this;
        }

        /// <summary>
        /// Repeats each element in the sequence delay times.
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        public Frames AddDelay(int delay)
        {
            var Result = this.SelectMany(x => Enumerable.Range(0, delay), (x, e) => x).ToArray();

            Clear();
            AddRange(Result);

            return this;
        }

        public static Frames Create(params int[] frames)
        {
            return new Frames(frames.ToList());
        }

        /// <summary>
        ///  Generates a sequence of integers within a specified range.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static Frames CreateRange(int start, int count)
        {
            var Range = Enumerable.Range(start, count);

            return new Frames(Range);
        }

        /// <summary>
        /// Adds a range similar to Enumerable.Range
        /// </summary>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public Frames AddRange(int start, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Add(i + start);
            }

            return this;
        }

        /// <summary>
        /// Returns an empty sequence.
        /// </summary>
        public static Frames Empty
        {
            get
            {
                return Create();
            }
        }

        /// <summary>
        /// Creates a copy of this sequence.
        /// </summary>
        /// <returns></returns>
        public Frames Copy()
        {
            return new Frames(this.ToList());
        }

        /// <summary>
        /// Returns an random element in this sequence
        /// </summary>
        /// <param name="randomizer"></param>
        /// <returns></returns>
        public int GetRandom(Randomizer randomizer)
        {
            var RandomIndex = randomizer.CreateInt(0, this.Count);

            return this[RandomIndex];
        }

        /// <summary>
        /// Returns a random element in this sequence not matching valueToExclude.
        /// </summary>
        /// <param name="randomizer"></param>
        /// <param name="valueToExclude"></param>
        /// <returns></returns>
        public int GetRandomExcluding(Randomizer randomizer, int valueToExclude)
        {
            if (this.Count == 1 && valueToExclude == this[0])
            {
                throw new InvalidOperationException("The only sequence value can't be excluded.");
            }

            var RandomResult = GetRandom(randomizer);

            while (RandomResult == valueToExclude)
            {
                RandomResult = GetRandom(randomizer);
            }

            return RandomResult;
        }
    }

    [Serializable]
    public class Sprite : Component
    {
        public const string EXISTINGTEXTUREIMAGE = "@ExistingTexture";
        public const string WHITEPIXELIMAGE = "@WhitePixel";
        public string Image { get; private set; }

        [NonSerialized]
        private Texture2D _Texture;
        public Texture2D Texture { get { return _Texture; } }

        [NonSerialized]
        private Texture2D _NormalMap;
        public Texture2D NormalMap { get { return _NormalMap; } }

        public Rectangle CurrentFrameRectangle { get; private set; }

        public float Depth { get; set; }
        public bool EnableNormalMap { get; private set; }
        public string NormalMapImage { get; private set; }
        public RenderStage RenderStage { get; set; }
        public Func<Vector2> GetPositionFn { get; set; }

        public Sprite()
        {
            Depth = 0;
            Columns = 1;
            Rows = 1;
            RenderStage = RenderStage.Bloom;
        }

        public int Rows { get; private set; }
        public int Columns { get; private set; }
        public int TotalFrames { get; private set; }

        private int _InitialFrame = 0;
        private int _CurrentFrame = -1;

        /// <summary>
        /// One based
        /// </summary>
        public int CurrentFrame
        {
            get
            {
                return _CurrentFrame + 1;
            }
            set
            {
                var NewValue = Math.Min(TotalFrames, Math.Max(0, value - 1));
                if (NewValue == _CurrentFrame)
                {
                    return;
                }
                _CurrentFrame = NewValue;

                int Row = (int)((float)_CurrentFrame / (float)Columns);
                int Column = _CurrentFrame % Columns;
                int Width = (Texture == null) ? 1 : Texture.Width / Columns;
                int Height = (Texture == null) ? 1 : Texture.Height / Rows;

                CurrentFrameRectangle = new Rectangle(Width * Column, Height * Row, Width, Height);
                _InitialFrame = _CurrentFrame;
            }
        }

        public override void OnLoadContent(ContentLoader content)
        {
            ImageCache = null;

            if (Image != WHITEPIXELIMAGE && Image != EXISTINGTEXTUREIMAGE && !string.IsNullOrEmpty(Image))
            {
                Log.WriteLine("Loading Sprite " + Image);

                _Texture = content.Load<Texture2D>(Image);

                if (EnableNormalMap)
                {
                    if (string.IsNullOrEmpty(NormalMapImage))
                    {
                        NormalMapImage = Image + "_normals";
                    }

                    Log.WriteLine("Loading NormalMap " + NormalMapImage);
                    _NormalMap = content.Load<Texture2D>(NormalMapImage);
                }
            }

            if (_CurrentFrame == -1)
            {
                CurrentFrame = _InitialFrame;
            }
        }

        public override void OnDraw(Renderer renderer)
        {
            if (RenderStage == renderer.Stage)
            {
                var Position = GetSpritePosition();

                Draw(renderer, Position);
            }
        }

        public void Draw(Renderer renderer, Vector2 position)
        {
            var Transform = Get<Transform>();

            if (Transform != null && Transform.Absolute)
            {
                var Camera = Entity.DrawScene.Get<Camera>();
                position = Camera.TransformInverse(position);
            }

            var Lightning = Entity.Get<Lightning>();

            if (NormalMap != null && Lightning != null)
            {
                renderer.End();

                renderer.ApplyNormalmapEffectParameter(Lightning, NormalMap);
                renderer.NormalmapEffect.Parameters["MatrixTransform"].SetValue(renderer.SpriteBatchProjection);
                renderer.Begin(renderer.Projection, null, null, renderer.NormalmapEffect);
            }

            if (Data != null && true)
            {
                var Scale = Data.Scale;
                if (Transform != null)
                {
                    Scale.X *= Transform.Scale;
                    Scale.Y *= Transform.Scale;
                }
                var pos = (position + Data.Offset * Scale);
                renderer.SpriteBatch.Draw(Texture ?? renderer.WhitePixelTexture, pos, CurrentFrameRectangle, Data.Color, Data.Rotation, Data.Origin, Scale, Data.Effects, Depth);
            }
            else
            {
                var pos = position;
                renderer.SpriteBatch.Draw(Texture ?? renderer.WhitePixelTexture, pos, CurrentFrameRectangle, Color.White);
            }

            if (NormalMap != null && Lightning != null)
            {
                renderer.End();
                renderer.Begin(renderer.Projection);
            }
        }

        private Vector2 GetSpritePosition()
        {
            if (null != GetPositionFn)
            {
                return GetPositionFn();
            }

            var Transform = Entity.Get<Transform>();
            if (Transform != null)
            {
                return Transform.Position;
            }

            return Vector2.Zero;
        }

        public bool IsRectangleHit(Vector2 point)
        {
            if (Texture == null)
            {
                return false;
            }

            var Position = GetSpritePosition();
            var ImagePosition = (point - Position);

            if (Data != null)
            {
                var Scale = Data.Scale;
                var Transform = Entity.Get<Transform>();
                if (Transform != null)
                {
                    Scale.X *= Transform.Scale;
                    Scale.Y *= Transform.Scale;
                }

                ImagePosition -= Data.Offset * Scale;
                ImagePosition /= Scale;
            }

            return new Rectangle(0, 0, Texture.Width / Columns, Texture.Height / Rows).Contains(ImagePosition);
        }

        /// <summary>
        /// Cache texture data to not load it from gpu each time.
        /// </summary>
        [NonSerialized]
        private Color[] ImageCache = null;

        public bool IsPixelHit(Vector2 point)
        {
            if (Texture == null)
            {
                return false;
            }

            var Position = GetSpritePosition();
            var ImagePosition = (point - Position);

            if (Data != null)
            {
                var Scale = Data.Scale;
                var Transform = Entity.Get<Transform>();
                if (Transform != null)
                {
                    Scale.X *= Transform.Scale;
                    Scale.Y *= Transform.Scale;
                }

                ImagePosition -= Data.Offset * Scale;
                ImagePosition /= Scale;
            }

            var SourceRectangle = new Rectangle((int)ImagePosition.X, (int)ImagePosition.Y, 1, 1);

            if (SourceRectangle.X < 0 || SourceRectangle.X > Texture.Width / Columns - 1 || SourceRectangle.Y < 0 || SourceRectangle.Y > Texture.Height / Rows - 1)
            {
                return false;
            }

            if (ImageCache == null)
            {
                ImageCache = new Color[Texture.Width * Texture.Height];
                Texture.GetData<Color>(ImageCache);
            }

            int index = (int)ImagePosition.X + CurrentFrameRectangle.Left + ((int)ImagePosition.Y + CurrentFrameRectangle.Top) * Texture.Width;

            return ImageCache[index].A > 40;
        }

        private SpriteData Data
        {
            get
            {
                return Get<SpriteData>();
            }
        }

        public float GetHeight()
        {
            var Transform = Get<Transform>();
            return (Texture == null) ? 0 : Texture.Height / Rows * (Data == null ? 1 : Data.Scale.Y) * (Transform == null ? 1 : Transform.Scale);
        }

        public float GetWidth()
        {
            var Transform = Get<Transform>();
            return (Texture == null) ? 0 : Texture.Width / Columns * (Data == null ? 1 : Data.Scale.X) * (Transform == null ? 1 : Transform.Scale);
        }

        void LoadSprite(string image, int columns = 1, int rows = 1, int totalFrames = 0, string normalMapImage = null)
        {
            Image = image;
            NormalMapImage = normalMapImage;

            if (Loaded)
            {
                OnLoadContent(Entity.UpdateScene.Content);
            }

            Rows = rows;
            Columns = columns;
            TotalFrames = totalFrames == 0 ? Rows * Columns : totalFrames;

            var Animation = Get<SpriteTransformAnimation>();
            if (null != Animation)
            {
                Animation.SetFrame();
            }
        }

        void LoadTexture(Texture2D texture, int columns = 1, int rows = 1, int totalFrames = 0, Texture2D normalMap = null)
        {
            if (null == texture)
            {
                throw new ArgumentNullException("Texture must not be null.");
            }
            _CurrentFrame = -1;
            _Texture = texture;
            _NormalMap = normalMap;
            LoadSprite(EXISTINGTEXTUREIMAGE, columns, rows, totalFrames, EXISTINGTEXTUREIMAGE);
        }

        public static Sprite Create(Entity addTo)
        {
            return addTo.Add<Sprite>();
        }

        public Sprite SetImage(string value, int columns = 1, int rows = 1, int totalFrames = 0, string normalMapImage = null) { LoadSprite(value, columns, rows, totalFrames, normalMapImage); return this; }
        public Sprite SetTexture(Texture2D value, int columns = 1, int rows = 1, int totalFrames = 0, Texture2D normalMapTexture = null) { LoadTexture(value, columns, rows, totalFrames, normalMapTexture); return this; }
        public Sprite SetRenderStage(RenderStage value) { RenderStage = value; return this; }
        public Sprite SetEnableNormalMap(bool value) { EnableNormalMap = value; return this; }
        public Sprite SetGetPositionFn(Func<Vector2> value) { GetPositionFn = value; return this; }
        /// <summary>
        /// 1 based
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Sprite SetFrame(int value) { _InitialFrame = value; return this; }
    }
}
