using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Spine;
using STACK.Components;
using System;
using System.IO;

namespace STACK.Graphics
{
    /// <summary>
    /// Provides classes and methods for rendering.
    /// </summary>
    public class Renderer
    {
        public Matrix TransformationMatrix = Matrix.Identity;
        public Matrix Projection = Matrix.Identity;

        public DisplaySettings DisplaySettings;

        public RenderStage Stage = RenderStage.PreBloom;
        public SpriteBatch SpriteBatch;
        public SkeletonMeshRenderer SkeletonRenderer;
        public PrimitivesRenderer PrimitivesRenderer;
        public GraphicsDevice GraphicsDevice;
        public Effect NormalmapEffect;
        public BloomComponent BloomEffect;
        public Texture2D WhitePixelTexture;
        public SpriteFont DefaultFont;

        RenderTarget2D CurrentRenderTarget;
        RenderTarget2D DrawBuffer;
        RenderTarget2D GUIBuffer;

        public Renderer(IServiceProvider services, ContentManager content, Point virtualResolution)
        {
            Log.WriteLine("Constructing renderer");
            GraphicsDevice = ((IGraphicsDeviceService)services.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;
            var RealResolution = new Point(GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);
            DisplaySettings = new DisplaySettings(virtualResolution, RealResolution);

            SkeletonRenderer = new SkeletonMeshRenderer(GraphicsDevice)
            {
                PremultipliedAlpha = false
            };

            PrimitivesRenderer = new PrimitivesRenderer(GraphicsDevice);
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            BloomEffect = new BloomComponent(GraphicsDevice);

            LoadContent(content);
        }

        /// <summary>
        /// Used for SpriteBatch vertex shaders.
        /// </summary>
        public Matrix SpriteBatchProjection
        {
            get
            {
                Matrix projection = new Matrix(
                    (float)(2.0 / (double)GraphicsDevice.Viewport.Width),
                    0.0f,
                    0.0f,
                    0.0f,
                    0.0f,
                    (float)(-2.0 / (double)GraphicsDevice.Viewport.Height),
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

        void LoadContent(ContentManager content)
        {
            Log.WriteLine("Loading renderer content");
            NormalmapEffect = content.Load<Effect>("shaders/normalmap");

            BloomEffect.LoadContent(content);

            DrawBuffer = new RenderTarget2D(GraphicsDevice,
                                    DisplaySettings.VirtualResolution.X, DisplaySettings.VirtualResolution.Y,
                                    true, SurfaceFormat.Color,
                                    DepthFormat.Depth24Stencil8, GraphicsDevice.PresentationParameters.MultiSampleCount,
                                    RenderTargetUsage.DiscardContents);

            GUIBuffer = new RenderTarget2D(GraphicsDevice,
                                    DisplaySettings.VirtualResolution.X, DisplaySettings.VirtualResolution.Y,
                                    true, SurfaceFormat.Color,
                                    DepthFormat.Depth24Stencil8, GraphicsDevice.PresentationParameters.MultiSampleCount,
                                    RenderTargetUsage.DiscardContents);

            CurrentRenderTarget = DrawBuffer;

            WhitePixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            WhitePixelTexture.SetData<Color>(new[] { Color.White });

            DefaultFont = content.Load<SpriteFont>("fonts/stack");
        }

        public void Dispose()
        {
            NormalmapEffect.Dispose();
            BloomEffect.UnloadContent();
            SpriteBatch.Dispose();
            PrimitivesRenderer.Dispose();
            WhitePixelTexture.Dispose();

            if (DrawBuffer != null)
            {
                DrawBuffer.Dispose();
                DrawBuffer = null;
            }

            if (CurrentRenderTarget != null)
            {
                CurrentRenderTarget.Dispose();
                CurrentRenderTarget = null;
            }
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

        private GameTime _NeoManagerGameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(GameSpeed.TickDuration), false);

        public void Update(KeyboardState ks, MouseState ms)
        {

        }

        private GameTime DefaultGameTime = new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(GameSpeed.TickDuration));

        public void Draw(World world)
        {
            if (world != null)
            {
                BloomEffect.Visible = world.Get<RenderSettings>().BloomEnabled;

                if (BloomEffect.Visible)
                {
                    BloomEffect.BeginDraw();
                    GraphicsDevice.Clear(Color.Black);
                    Stage = RenderStage.Bloom;

                    world.Draw(this);

                    BloomEffect.Draw(world.Get<RenderSettings>().BloomSettings);
                }

                GraphicsDevice.SetRenderTarget(CurrentRenderTarget);

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

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, DisplaySettings.ScaleMatrix);
            GraphicsDevice.Viewport = DisplaySettings.Viewport;
            SpriteBatch.Draw(DrawBuffer, new Vector2(0, 0), Color.White);
            SpriteBatch.End();
        }

        /// <summary>
        /// Begins the SpriteBatch using the given projection matrix.
        /// </summary>        
        public void Begin(Matrix projection, BlendState blendState = null, SamplerState samplerState = null, Effect effect = null)
        {
            SpriteBatch.Begin(SpriteSortMode.Immediate, blendState ?? BlendState.NonPremultiplied, samplerState ?? SamplerState.PointClamp, null, null, effect, projection * TransformationMatrix);
            PrimitivesRenderer.SetTransformation(projection);
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
            RenderTarget2D ScreenShot = new RenderTarget2D(GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight);

            GraphicsDevice.SetRenderTarget(ScreenShot);
            CurrentRenderTarget = ScreenShot;

            Draw(world);

            GraphicsDevice.SetRenderTarget(null);
            CurrentRenderTarget = DrawBuffer;

            return ScreenShot;
        }

        private byte[] GetTexturePNGData(Texture2D screenshot)
        {
            using (var Writer = new MemoryStream())
            {
                screenshot.SaveAsPng(Writer, screenshot.Width, screenshot.Height);

                return Writer.ToArray();
            }
        }

        public byte[] GetScreenshotPNGData(World world)
        {
            using (var ScreenShot = GetScreenshot(world))
            {
                return GetTexturePNGData(ScreenShot);
            }
        }

        public void SaveScreenShot(World world)
        {
            int index = 0;

            string name = "Screenshot" + index + ".png";

            while (File.Exists(name))
            {
                index++;
                name = "Screenshot" + index + ".png";
            }

            using (var ScreenShot = GetScreenshot(world))
            {
                using (var Stream = new FileStream(name, FileMode.Create))
                {
                    ScreenShot.SaveAsPng(Stream, ScreenShot.Width, ScreenShot.Height);
                }
            }

            Log.WriteLine("Screenshot saved to file " + name);
        }
    }
}
