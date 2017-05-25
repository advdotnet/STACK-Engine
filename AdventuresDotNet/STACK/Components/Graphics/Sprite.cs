using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using STACK.Graphics;
using System;

namespace STACK.Components
{
    [Serializable]
    public class Sprite : Component
    {
        public const string WHITEPIXELIMAGE = "WhitePixel";
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
        public RenderStage RenderStage { get; set; }        

        public Sprite()
        {
            Depth = 0;                        
            RenderStage = RenderStage.Bloom;
        }

		public int Rows { get; private set; }
		public int Columns { get; private set; }

		private int _InitialFrame = 0;

		private int _CurrentFrame;

		public int CurrentFrame 
		{ 
			get 
			{
				return _CurrentFrame + 1;
			}
			set 
			{
				_CurrentFrame = Math.Min(TotalFrames, Math.Max(0, value-1));

				int Row = (int)((float)_CurrentFrame / (float)Columns);
				int Column = _CurrentFrame % Columns;
				int Width = (Texture == null) ? 1 : Texture.Width / Columns;
				int Height = (Texture == null) ? 1 : Texture.Height / Rows;

				CurrentFrameRectangle = new Rectangle(Width * Column, Height * Row, Width, Height);
				_InitialFrame = _CurrentFrame;
			}
		}

		public int TotalFrames { get; private set; }

        public override void OnLoadContent(ContentLoader content)
        {
            ImageCache = null;

            if (Image != WHITEPIXELIMAGE)
            {
                Log.WriteLine("Loading Sprite " + Image);
                _Texture = content.Load<Texture2D>(Image);

				if (EnableNormalMap)
				{
					Log.WriteLine("Loading NormalMap " + Image + "_NORMALS");
					_NormalMap = content.Load<Texture2D>(Image + "_NORMALS");
				}				
            }

            CurrentFrame = _InitialFrame;
        }

        public override void OnDraw(Renderer renderer)
        {
            if (RenderStage.HasFlag(renderer.Stage))
            {
                Vector2 Position = Vector2.Zero;

                var Transform = Entity.Get<Transform>();
                if (Transform != null)
                {
                    Position = Transform.Position;
                }
                
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
                
                renderer.SpriteBatch.Draw(Texture ?? renderer.WhitePixelTexture, position + Data.Offset * Scale, CurrentFrameRectangle, Data.Color, Data.Rotation, Data.Origin, Scale, Data.Effects, Depth);                
            }
			else
			{
				renderer.SpriteBatch.Draw(Texture ?? renderer.WhitePixelTexture, position, CurrentFrameRectangle, Color.White);
			}

			if (NormalMap != null && Lightning != null)
			{
				renderer.End();
				renderer.Begin(renderer.Projection);
			}
        }

        public bool IsRectangleHit(Vector2 point)
        {
            Vector2 Position = Vector2.Zero;

            var Transform = Entity.Get<Transform>();
            if (Transform != null)
            {
                Position = Transform.Position;
            }            

            if (Texture == null)
            {
                return false;
            }

            return new Rectangle(0, 0, Texture.Width / Columns, Texture.Height / Rows).Contains(point - Position);
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

            var Position = Vector2.Zero;

            var Transform = Entity.Get<Transform>();
            if (Transform != null)
            {
                Position = Transform.Position;
            }			

            var ImagePosition = (point - Position);

            if (Data != null)
            {
				var Scale = Data.Scale;
				if (Transform != null)
				{
					Scale.X *= Transform.Scale;
					Scale.Y *= Transform.Scale;
				}

                ImagePosition -= Data.Offset * Scale;
                ImagePosition /= Scale;
            }

            Rectangle SourceRectangle = new Rectangle((int)ImagePosition.X, (int)ImagePosition.Y, 1, 1);

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

        void LoadSprite(string image, int columns = 1, int rows = 1, int totalFrames = 0) 
        {            
            Image = image;
            if (Loaded)
            {
                OnLoadContent(Entity.UpdateScene.Content);
            }

			Rows = rows;
			Columns = columns;
			TotalFrames = totalFrames == 0 ? Rows * Columns : totalFrames;
        }		

        public static Sprite Create(Entity addTo)
        {
            return addTo.Add<Sprite>();            
        }

		public Sprite SetImage(string value, int columns = 1, int rows = 1, int totalFrames = 0) { LoadSprite(value, columns, rows, totalFrames); return this; }
        public Sprite SetRenderStage(RenderStage value) { RenderStage = value; return this; }
		public Sprite SetEnableNormalMap(bool value) { EnableNormalMap = value; return this; }
		public Sprite SetFrame(int value) { _InitialFrame = value - 1; return this; }		
    }  
}
