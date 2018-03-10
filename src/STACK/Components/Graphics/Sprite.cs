using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using STACK.Graphics;
using STACK.Logging;
using System;

namespace STACK.Components
{
    [Serializable]
    public class Sprite : Component, IContent, IDraw
    {
        public const string EXISTINGTEXTUREIMAGE = "@ExistingTexture";
        public const string WHITEPIXELIMAGE = "@WhitePixel";

        [NonSerialized]
        Texture2D _Texture;
        [NonSerialized]
        Texture2D _NormalMap;
        [NonSerialized]
        bool Loaded = false;
        bool _Visible;
        float _DrawOrder;
        string _Image;
        Rectangle _CurrentFrameRectangle;
        float _Depth;
        bool _EnableNormalMap;
        string _NormalMapImage;
        RenderStage _RenderStage;
        Func<Vector2> _GetPositionFn;
        int _Rows;
        int _Columns;
        int _TotalFrames;
        int _InitialFrame = 0;
        int _CurrentFrame = -1;
        /// <summary>
        /// Cache texture data to not load it from gpu each time.
        /// </summary>
        [NonSerialized]
        Color[] ImageCache = null;

        public bool Visible { get { return _Visible; } set { _Visible = value; } }
        public float DrawOrder { get { return _DrawOrder; } set { _DrawOrder = value; } }
        public string Image { get { return _Image; } }
        public Texture2D Texture { get { return _Texture; } }
        public Texture2D NormalMap { get { return _NormalMap; } }
        public Rectangle CurrentFrameRectangle { get { return _CurrentFrameRectangle; } }
        public float Depth { get { return _Depth; } set { _Depth = value; } }
        public bool EnableNormalMap { get { return _EnableNormalMap; } }
        public string NormalMapImage { get { return _NormalMapImage; } }
        public RenderStage RenderStage { get { return _RenderStage; } set { _RenderStage = value; } }
        public Func<Vector2> GetPositionFn { get { return _GetPositionFn; } set { _GetPositionFn = value; } }
        public int Rows { get { return _Rows; } set { _Rows = value; } }
        public int Columns { get { return _Columns; } set { _Columns = value; } }
        public int TotalFrames { get { return _TotalFrames; } set { _TotalFrames = value; } }

        public Sprite()
        {
            Depth = 0;
            Columns = 1;
            Rows = 1;
            RenderStage = RenderStage.Bloom;
            Visible = true;
        }

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

                _CurrentFrameRectangle = new Rectangle(Width * Column, Height * Row, Width, Height);
                _InitialFrame = _CurrentFrame;
            }
        }

        public void LoadContent(ContentLoader content)
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
                        _NormalMapImage = Image + "_normals";
                    }

                    Log.WriteLine("Loading NormalMap " + NormalMapImage);
                    _NormalMap = content.Load<Texture2D>(NormalMapImage);
                }
            }

            if (_CurrentFrame == -1)
            {
                CurrentFrame = _InitialFrame;
            }

            Loaded = true;
        }

        public void UnloadContent() { }

        public void Draw(Renderer renderer)
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
            _Image = image;
            _NormalMapImage = normalMapImage;

            if (Loaded)
            {
                LoadContent(Entity.UpdateScene.Content);
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
        public Sprite SetEnableNormalMap(bool value) { _EnableNormalMap = value; return this; }
        public Sprite SetGetPositionFn(Func<Vector2> value) { GetPositionFn = value; return this; }
        /// <summary>
        /// 1 based
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Sprite SetFrame(int value) { _InitialFrame = value; return this; }
        public Sprite SetVisible(bool value) { Visible = value; return this; }
    }
}
