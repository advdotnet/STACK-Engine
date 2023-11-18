using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using STACK.Components;
using STACK.Logging;
using System;
using System.IO;
using System.Threading;
using TomShane.Neoforce.Controls;

namespace STACK.Graphics
{
	/// <summary>
	/// Provides classes and methods for rendering.
	/// </summary>
	public class Renderer
	{
		public Matrix TransformationMatrix = Matrix.Identity;
		public Matrix Projection = Matrix.Identity;
		public bool BloomEnabled = true;

		public DisplaySettings DisplaySettings;

		public RenderStage Stage = RenderStage.PreBloom;
		public SpriteBatch SpriteBatch;
		public PrimitivesRenderer PrimitivesRenderer;
		public GraphicsDevice GraphicsDevice;
		public Effect NormalmapEffect;
		public BloomComponent BloomEffect;
		public Texture2D WhitePixelTexture;
		private Manager _neoManager;
		private RenderTarget2D _currentRenderTarget;
		private RenderTarget2D _drawBuffer;
		private RenderTarget2D _guiBuffer;

		public Renderer(IServiceProvider services, ContentManager content, Point virtualResolution, Point? targetResolution = null, bool bloom = true)
		{
			Log.WriteLine("Constructing renderer");
			GraphicsDevice = ((IGraphicsDeviceService)services.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;
			var realResolution = new Point(GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);
			DisplaySettings = new DisplaySettings(virtualResolution, realResolution, targetResolution);
			BloomEnabled = bloom;
			PrimitivesRenderer = new PrimitivesRenderer(GraphicsDevice);
			SpriteBatch = new SpriteBatch(GraphicsDevice);
			BloomEffect = new BloomComponent(GraphicsDevice);

			_neoManager = new Manager(services, GraphicsDevice, "Default")
			{
				AutoCreateRenderTarget = true,
				TargetFrames = 60,
				LogUnhandledExceptions = false,
				ShowSoftwareCursor = false
			};

			if ("de-DE" == Thread.CurrentThread.CurrentUICulture.Name)
			{
				GUIManager.KeyboardLayout = new GermanKeyboardLayout();
			}

			LoadContent(content);
		}

		public Manager GUIManager
		{
			get => _neoManager;

			set => _neoManager = value;
		}

		/// <summary>
		/// Used for SpriteBatch vertex shaders.
		/// </summary>
		public Matrix SpriteBatchProjection
		{
			get
			{
				var projection = new Matrix(
					(float)(2.0 / GraphicsDevice.Viewport.Width),
					0.0f,
					0.0f,
					0.0f,
					0.0f,
					(float)(-2.0 / GraphicsDevice.Viewport.Height),
					0.0f,
					0.0f,
					0.0f,
					0.0f,
					1.0f,
					0.0f,
					-1.0f,
					1.0f,
					0.0f,
					1.0f
				);

				return Matrix.Multiply(Projection, projection);
			}
		}

		public SpriteFont DefaultFont;

		private void LoadContent(ContentManager content)
		{
			Log.WriteLine("Loading renderer content");
			NormalmapEffect = content.Load<Effect>(STACK.content.shaders.Normalmap);

			BloomEffect.LoadContent(content);

			_drawBuffer = new RenderTarget2D(GraphicsDevice,
									DisplaySettings.VirtualResolution.X, DisplaySettings.VirtualResolution.Y,
									true, SurfaceFormat.Color,
									DepthFormat.Depth24Stencil8, GraphicsDevice.PresentationParameters.MultiSampleCount,
									RenderTargetUsage.DiscardContents);

			_guiBuffer = new RenderTarget2D(GraphicsDevice,
									DisplaySettings.VirtualResolution.X, DisplaySettings.VirtualResolution.Y,
									true, SurfaceFormat.Color,
									DepthFormat.Depth24Stencil8, GraphicsDevice.PresentationParameters.MultiSampleCount,
									RenderTargetUsage.DiscardContents);

			_currentRenderTarget = _drawBuffer;

			WhitePixelTexture = new Texture2D(GraphicsDevice, 1, 1);
			WhitePixelTexture.SetData(new[] { Color.White });

			DefaultFont = content.Load<SpriteFont>(STACK.content.fonts.stack);

			_neoManager.AutoCreateRenderTarget = false;
			_neoManager.RenderTarget = _guiBuffer;
			_neoManager.Initialize();
		}

		public void Dispose()
		{
			NormalmapEffect.Dispose();
			BloomEffect.UnloadContent();
			SpriteBatch.Dispose();
			PrimitivesRenderer.Dispose();
			WhitePixelTexture.Dispose();

			if (_drawBuffer != null)
			{
				_drawBuffer.Dispose();
				_drawBuffer = null;
			}

			if (_currentRenderTarget != null)
			{
				_currentRenderTarget.Dispose();
				_currentRenderTarget = null;
			}
			_neoManager.Dispose(true);
		}

		public void ApplyNormalmapEffectParameter(Lightning settings, Texture2D normalMap)
		{
			NormalmapEffect.Parameters["MatrixTransform"].SetValue(Projection * TransformationMatrix);
			NormalmapEffect.Parameters["LightPosition"].SetValue(settings.LightPosition);
			NormalmapEffect.Parameters["LightColor"].SetValue(settings.LightColor);
			NormalmapEffect.Parameters["AmbientColor"].SetValue(settings.AmbientColor);
			NormalmapEffect.Parameters["DrawNormals"].SetValue(settings.DrawNormals);

			if (EngineVariables.DrawNormals)
			{
				NormalmapEffect.Parameters["DrawNormals"].SetValue(1f);
			}

			NormalmapEffect.Parameters["CellShading"].SetValue(settings.CellShading);
			//NormalmapEffect.Parameters["EnableRotation"].SetValue(settings.EnableRotation);			

			// FNA
			//NormalmapEffect.Parameters["NormalSampler"].SetValue(normalMap);            
			GraphicsDevice.Textures[1] = normalMap;
		}

		private readonly GameTime _neoManagerGameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(GameSpeed.TickDuration), false);

		public void Update(KeyboardState ks, MouseState ms)
		{
			if (!_neoManager.Disposing && _neoManager.ShowSoftwareCursor && EngineVariables.EnableGUI)
			{
				// FNA
				//_NeoManagerGameTime.ElapsedGameTime = TimeSpan.FromSeconds(GameSpeed.TickDuration);
				_neoManager.Update(_neoManagerGameTime, ks, ms);
			}
		}

		private readonly GameTime _defaultGameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(GameSpeed.TickDuration));

		public void Draw(World world)
		{
			if ((_currentRenderTarget == null || _currentRenderTarget == _drawBuffer) && _neoManager.ShowSoftwareCursor && EngineVariables.EnableGUI)
			{
				_neoManager.BeginDraw(_defaultGameTime);
				GraphicsDevice.SetRenderTarget(_currentRenderTarget);
			}

			if (world != null)
			{
				BloomEffect.Visible = (BloomEnabled && world.Get<RenderSettings>().BloomEnabled);

				if (BloomEffect.Visible)
				{
					BloomEffect.BeginDraw();
					GraphicsDevice.Clear(Color.Black);
					Stage = RenderStage.Bloom;

					world.Draw(this);

					BloomEffect.Draw(world.Get<RenderSettings>().BloomSettings);
				}

				GraphicsDevice.SetRenderTarget(_currentRenderTarget);

				Stage = RenderStage.PreBloom;

				world.Draw(this);

				if (!BloomEffect.Visible)
				{
					Stage = RenderStage.Bloom;

					world.Draw(this);
				}
				else
				{
					BloomEffect.Flush();
				}

				Stage = RenderStage.PostBloom;
				world.Draw(this);
			}

			if ((_currentRenderTarget == null || _currentRenderTarget == _drawBuffer) && _neoManager.ShowSoftwareCursor && EngineVariables.EnableGUI)
			{
				_neoManager.EndDraw();
			}

			GraphicsDevice.SetRenderTarget(null);
			GraphicsDevice.Clear(Color.Black);

			SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null, null, DisplaySettings.ScaleMatrix);
			GraphicsDevice.Viewport = DisplaySettings.Viewport;
			SpriteBatch.Draw(_drawBuffer, Vector2.Zero, Color.White);
			SpriteBatch.End();
		}

		/// <summary>
		/// Begins the SpriteBatch using the given projection matrix.
		/// </summary>        
		public void Begin(Matrix projection, BlendState blendState = null, SamplerState samplerState = null, Effect effect = null)
		{
			SpriteBatch.Begin(SpriteSortMode.Immediate, blendState ?? BlendState.NonPremultiplied, samplerState ?? SamplerState.PointClamp, null, null, effect, projection * TransformationMatrix);
			PrimitivesRenderer.SetTransformation(projection * DisplaySettings.ScaleMatrix);
			Projection = projection;
		}

		/// <summary>
		/// Flushes the SpriteBatch.
		/// </summary>
		public void End()
		{
			SpriteBatch.End();
		}

		private Texture2D GetScreenshot(World world)
		{
			var screenShot = new RenderTarget2D(GraphicsDevice,
				GraphicsDevice.PresentationParameters.BackBufferWidth,
				GraphicsDevice.PresentationParameters.BackBufferHeight);

			GraphicsDevice.SetRenderTarget(screenShot);
			_currentRenderTarget = screenShot;

			Draw(world);

			GraphicsDevice.SetRenderTarget(null);
			_currentRenderTarget = _drawBuffer;

			return screenShot;
		}

		private byte[] GetTexturePNGData(Texture2D screenshot)
		{
			using (var writer = new MemoryStream())
			{
				screenshot.SaveAsPng(writer, screenshot.Width, screenshot.Height);

				return writer.ToArray();
			}
		}

		public byte[] GetScreenshotPNGData(World world)
		{
			using (var screenShot = GetScreenshot(world))
			{
				return GetTexturePNGData(screenShot);
			}
		}

		public void SaveScreenShot(World world)
		{
			var index = 0;

			var name = "Screenshot" + index + ".png";

			while (File.Exists(name))
			{
				index++;
				name = "Screenshot" + index + ".png";
			}

			using (var screenShot = GetScreenshot(world))
			{
				using (var stream = new FileStream(name, FileMode.Create))
				{
					screenShot.SaveAsPng(stream, screenShot.Width, screenShot.Height);
				}
			}

			Log.WriteLine("Screenshot saved to file " + name);
		}
	}
}
