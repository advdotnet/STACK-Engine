using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using STACK.Input;
using STACK.Utils;
using System;

namespace STACK
{
    public partial class Window : Game, ISkipContent
    {
        public StackEngine StackEngine { get; private set; }
        public SkipText SkipText { get; }
        public SkipCutscene SkipCutscene { get; }
        GameSpeed GameSpeed = GameSpeed.Default;
        InputProvider InputProvider;
        GraphicsDeviceManager Graphics;
        StackGame Game;
        FrameRateCounter Counter;

        public Window(StackGame game) : base()
        {
            Game = game;

            Log.AddLogger(new DebugLogHandler());
            Log.WriteLine("Initializing GraphicsDeviceManager");

            Graphics = new GraphicsDeviceManager(this);
            Graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(SetToReference);
            Window.ClientSizeChanged += OnClientSizeChanged;
            Window.AllowUserResizing = false;
            Window.Title = game.Title;

            if (!Graphics.GraphicsDevice.Adapter.IsProfileSupported(GraphicsProfile.HiDef) ||
                !Graphics.GraphicsDevice.Adapter.IsProfileSupported(GraphicsProfile.Reach))
            {
                throw new Exception("Graphic profile not supported.");
            }

            SkipText = new SkipText();
            SkipCutscene = new SkipCutscene(SetSpeed);
        }

        void SetToReference(object sender, PreparingDeviceSettingsEventArgs eventargs)
        {
            eventargs.GraphicsDeviceInformation.PresentationParameters.PresentationInterval = PresentInterval.One;
            eventargs.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.DiscardContents;
        }

        protected override void Initialize()
        {
            Graphics.PreferMultiSampling = false;
            Graphics.SynchronizeWithVerticalRetrace = true;
            Graphics.IsFullScreen = EngineVariables.Fullscreen;

            Graphics.PreferredBackBufferWidth = Game.VirtualResolution.X * Game.ResolutionScaleFactor.X;
            Graphics.PreferredBackBufferHeight = Game.VirtualResolution.Y * Game.ResolutionScaleFactor.Y;

            IsFixedTimeStep = true;
            IsMouseVisible = false;

            Graphics.ApplyChanges();

            var Services = new GameServiceContainer();
            Services.AddService(typeof(IGraphicsDeviceService), Graphics);
            Services.AddService(typeof(ISkipContent), this);

            InputProvider = new UserInputProvider();
            StackEngine = new StackEngine(Game, Services, InputProvider);
            InputProvider.Handler += HandleDebugInputEvent;
            StackEngine.OnExit += Exit;

            Counter = new FrameRateCounter();
            SetSpeed(GameSpeed.Default);

            base.Initialize();
        }

        private void OnClientSizeChanged(object sender, EventArgs eventArgs)
        {
            if (StackEngine != null && StackEngine.Renderer != null)
            {
                StackEngine.Renderer.DisplaySettings.OnClientSizeChanged(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            }
        }

        protected override void UnloadContent()
        {
            StackEngine.Exit();
        }

        protected override void Update(GameTime time)
        {
            Counter.UpdateStart();
            StackEngine.Update();
            Counter.UpdateEnd();

            if (SkipCutscene.Enabled)
            {
                SuppressDraw();
            }
        }

        protected override void Draw(GameTime time)
        {
            StackEngine.Draw();

            if (EngineVariables.ShowFPS)
            {
                Counter.Draw(StackEngine.Renderer.SpriteBatch, StackEngine.Renderer.DefaultFont, Vector2.Zero);
            }
        }

        public void SetSpeed(GameSpeed speed)
        {
            Log.WriteLine("Setting game speed to " + speed.Text);
            GameSpeed = speed;

            TargetElapsedTime = speed.TargetElapsedTime;
            // FNA
            //MaxElapsedTime = speed.MaxElapsedTime;            
        }
    }
}
