using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using STACK.Graphics;
using STACK.Input;
using STACK.Logging;
using STACK.Utils;
using System;

namespace STACK
{
    public partial class Window : Game, ISkipContent
    {
        public GraphicSettings GraphicSettings { get; private set; }
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
            Log.WriteLine("Loading graphic settings");

            GraphicSettings = GraphicSettings.LoadFromConfigFile();

            Log.WriteLine("Initializing graphics");

            Graphics = GraphicSettings.CreateGraphicsDeviceManager(this);

            Window.ClientSizeChanged += OnClientSizeChanged;
            Window.AllowUserResizing = true;
            Window.Title = game.Title;

            SkipText = new SkipText();
            SkipCutscene = new SkipCutscene(SetSpeed);
        }

        protected override void Initialize()
        {
            IsFixedTimeStep = true;
            IsMouseVisible = false;

            GraphicSettings.Initialize(Graphics);

            var Services = new GameServiceContainer();
            Services.AddService(typeof(IGraphicsDeviceService), Graphics);
            Services.AddService(typeof(ISkipContent), this);

            InputProvider = new UserInputProvider();
            StackEngine = new StackEngine(Game, Services, InputProvider, GraphicSettings.GetTargetResolution());
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

        protected override void Update(GameTime time)
        {
            if (EngineVariables.ShowFPS)
            {
                Counter.UpdateStart();
            }

            StackEngine.Update();

            if (EngineVariables.ShowFPS)
            {
                Counter.UpdateEnd();
            }

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
            if (!GameSpeed.Equals(speed))
            {
                Log.WriteLine("Setting game speed to " + speed.Text);
                GameSpeed = speed;
                TargetElapsedTime = speed.TargetElapsedTime;
            }
        }
    }
}
