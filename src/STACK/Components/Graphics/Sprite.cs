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
		private Texture2D _texture;
		[NonSerialized]
		private Texture2D _normalMap;
		[NonSerialized]
		private bool _loaded = false;
		private bool _visible;
		private float _drawOrder;
		private string _image;
		private Rectangle _currentFrameRectangle;
		private float _depth;
		private bool _enableNormalMap;
		private string _normalMapImage;
		private RenderStage _renderStage;
		private Func<Vector2> _getPositionFn;
		private int _rows;
		private int _columns;
		private int _totalFrames;
		private int _initialFrame = 0;
		private int _currentFrame = -1;
		/// <summary>
		/// Cache texture data to not load it from gpu each time.
		/// </summary>
		[NonSerialized]
		private Color[] _imageCache = null;

		public bool Visible { get => _visible; set => _visible = value; }
		public float DrawOrder { get => _drawOrder; set => _drawOrder = value; }
		public string Image => _image;
		public Texture2D Texture => _texture;
		public Texture2D NormalMap => _normalMap;
		public Rectangle CurrentFrameRectangle => _currentFrameRectangle;
		public float Depth { get => _depth; set => _depth = value; }
		public bool EnableNormalMap => _enableNormalMap;
		public string NormalMapImage => _normalMapImage;
		public RenderStage RenderStage { get => _renderStage; set => _renderStage = value; }
		public Func<Vector2> GetPositionFn { get => _getPositionFn; set => _getPositionFn = value; }
		public int Rows { get => _rows; set => _rows = value; }
		public int Columns { get => _columns; set => _columns = value; }
		public int TotalFrames { get => _totalFrames; set => _totalFrames = value; }

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
			get => _currentFrame + 1;
			set
			{
				var newValue = Math.Min(TotalFrames, Math.Max(0, value - 1));
				if (newValue == _currentFrame)
				{
					return;
				}
				_currentFrame = newValue;

				var row = (int)((float)_currentFrame / (float)Columns);
				var column = _currentFrame % Columns;
				var width = (Texture == null) ? 1 : Texture.Width / Columns;
				var height = (Texture == null) ? 1 : Texture.Height / Rows;

				_currentFrameRectangle = new Rectangle(width * column, height * row, width, height);
				_initialFrame = _currentFrame;
			}
		}

		public void LoadContent(ContentLoader content)
		{
			_imageCache = null;

			if (Image != WHITEPIXELIMAGE && Image != EXISTINGTEXTUREIMAGE && !string.IsNullOrEmpty(Image))
			{
				Log.WriteLine("Loading Sprite " + Image);

				_texture = content.Load<Texture2D>(Image);

				if (EnableNormalMap)
				{
					if (string.IsNullOrEmpty(NormalMapImage))
					{
						_normalMapImage = Image + "_normals";
					}

					Log.WriteLine("Loading NormalMap " + NormalMapImage);
					_normalMap = content.Load<Texture2D>(NormalMapImage);
				}
			}

			if (_currentFrame == -1)
			{
				CurrentFrame = _initialFrame;
			}

			_loaded = true;
		}

		public void UnloadContent() { }

		public void Draw(Renderer renderer)
		{
			if (RenderStage == renderer.Stage)
			{
				var position = GetSpritePosition();

				Draw(renderer, position);
			}
		}

		public void Draw(Renderer renderer, Vector2 position)
		{
			var transform = Get<Transform>();

			if (transform != null && transform.Absolute)
			{
				var camera = Entity.DrawScene.Get<Camera>();
				position = camera.TransformInverse(position);
			}

			var lightning = Entity.Get<Lightning>();

			if (NormalMap != null && lightning != null)
			{
				renderer.End();

				renderer.ApplyNormalmapEffectParameter(lightning, NormalMap);
				renderer.NormalmapEffect.Parameters["MatrixTransform"].SetValue(renderer.SpriteBatchProjection);
				renderer.Begin(renderer.Projection, null, null, renderer.NormalmapEffect);
			}

			if (Data != null && true)
			{
				var scale = Data.Scale;
				if (transform != null)
				{
					scale.X *= transform.Scale;
					scale.Y *= transform.Scale;
				}
				var pos = (position + Data.Offset * scale);
				renderer.SpriteBatch.Draw(Texture ?? renderer.WhitePixelTexture, pos, CurrentFrameRectangle, Data.Color, Data.Rotation, Data.Origin, scale, Data.Effects, Depth);
			}
			else
			{
				var pos = position;
				renderer.SpriteBatch.Draw(Texture ?? renderer.WhitePixelTexture, pos, CurrentFrameRectangle, Color.White);
			}

			if (NormalMap != null && lightning != null)
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

			var transform = Entity.Get<Transform>();

			return transform != null ? transform.Position : Vector2.Zero;
		}

		public bool IsRectangleHit(Vector2 point)
		{
			if (Texture == null)
			{
				return false;
			}

			var position = GetSpritePosition();
			var imagePosition = (point - position);

			if (Data != null)
			{
				var scale = Data.Scale;
				var transform = Entity.Get<Transform>();
				if (transform != null)
				{
					scale.X *= transform.Scale;
					scale.Y *= transform.Scale;
				}

				imagePosition -= Data.Offset * scale;
				imagePosition /= scale;
			}

			return new Rectangle(0, 0, Texture.Width / Columns, Texture.Height / Rows).Contains(imagePosition);
		}

		public bool IsPixelHit(Vector2 point)
		{
			if (Texture == null)
			{
				return false;
			}

			var position = GetSpritePosition();
			var imagePosition = (point - position);

			if (Data != null)
			{
				var scale = Data.Scale;
				var transform = Entity.Get<Transform>();
				if (transform != null)
				{
					scale.X *= transform.Scale;
					scale.Y *= transform.Scale;
				}

				imagePosition -= Data.Offset * scale;
				imagePosition /= scale;
			}

			var sourceRectangle = new Rectangle((int)imagePosition.X, (int)imagePosition.Y, 1, 1);

			if (sourceRectangle.X < 0 || sourceRectangle.X > Texture.Width / Columns - 1 || sourceRectangle.Y < 0 || sourceRectangle.Y > Texture.Height / Rows - 1)
			{
				return false;
			}

			if (_imageCache == null)
			{
				_imageCache = new Color[Texture.Width * Texture.Height];
				Texture.GetData(_imageCache);
			}

			var index = (int)imagePosition.X + CurrentFrameRectangle.Left + ((int)imagePosition.Y + CurrentFrameRectangle.Top) * Texture.Width;

			return _imageCache[index].A > 40;
		}

		private SpriteData Data => Get<SpriteData>();

		public float GetHeight()
		{
			var transform = Get<Transform>();
			return (Texture == null) ? 0 : Texture.Height / Rows * (Data == null ? 1 : Data.Scale.Y) * (transform == null ? 1 : transform.Scale);
		}

		public float GetWidth()
		{
			var transform = Get<Transform>();
			return (Texture == null) ? 0 : Texture.Width / Columns * (Data == null ? 1 : Data.Scale.X) * (transform == null ? 1 : transform.Scale);
		}

		private void LoadSprite(string image, int columns = 1, int rows = 1, int totalFrames = 0, string normalMapImage = null)
		{
			_image = image;
			_normalMapImage = normalMapImage;

			if (_loaded)
			{
				LoadContent(Entity.UpdateScene.Content);
			}

			Rows = rows;
			Columns = columns;
			TotalFrames = totalFrames == 0 ? Rows * Columns : totalFrames;

			var animation = Get<SpriteTransformAnimation>();
			animation?.SetFrame();
		}

		private void LoadTexture(Texture2D texture, int columns = 1, int rows = 1, int totalFrames = 0, Texture2D normalMap = null)
		{
			_texture = texture ?? throw new ArgumentNullException("Texture must not be null.");
			_currentFrame = -1;
			_normalMap = normalMap;
			LoadSprite(EXISTINGTEXTUREIMAGE, columns, rows, totalFrames, EXISTINGTEXTUREIMAGE);
		}

		public static Sprite Create(Entity addTo)
		{
			return addTo.Add<Sprite>();
		}

		public Sprite SetImage(string value, int columns = 1, int rows = 1, int totalFrames = 0, string normalMapImage = null) { LoadSprite(value, columns, rows, totalFrames, normalMapImage); return this; }
		public Sprite SetTexture(Texture2D value, int columns = 1, int rows = 1, int totalFrames = 0, Texture2D normalMapTexture = null) { LoadTexture(value, columns, rows, totalFrames, normalMapTexture); return this; }
		public Sprite SetRenderStage(RenderStage value) { RenderStage = value; return this; }
		public Sprite SetEnableNormalMap(bool value) { _enableNormalMap = value; return this; }
		public Sprite SetGetPositionFn(Func<Vector2> value) { GetPositionFn = value; return this; }
		/// <summary>
		/// 1 based
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public Sprite SetFrame(int value) { _initialFrame = value; return this; }
		public Sprite SetVisible(bool value) { Visible = value; return this; }
	}
}
