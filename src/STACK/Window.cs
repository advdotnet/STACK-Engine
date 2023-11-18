using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using STACK.Input;
using STACK.Logging;
using STACK.Utils;
using System;

namespace STACK
{
	public partial class Window : Game, ISkipContent
	{
		public GameSettings GameSettings { get; private set; }
		public StackEngine StackEngine { get; private set; }
		public SkipText SkipText { get; }
		public SkipCutscene SkipCutscene { get; }

		private GameSpeed _gameSpeed = GameSpeed.Default;
		private InputProvider _inputProvider;
		private readonly GraphicsDeviceManager _graphics;
		private readonly StackGame _game;
		private FrameRateCounter _counter;

		public Window(StackGame game) : base()
		{
			_game = game;

			Log.AddLogger(new DebugLogHandler());
			Log.WriteLine("Loading game settings");

			GameSettings = GameSettings.LoadFromConfigFile(_game.SaveGameFolder);

			Log.WriteLine("Initializing graphics");

			_graphics = GameSettings.CreateGraphicsDeviceManager(this);

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

			GameSettings.Initialize(_graphics, _game.VirtualResolution);

			var services = new GameServiceContainer();
			services.AddService(typeof(IGraphicsDeviceService), _graphics);
			services.AddService(typeof(ISkipContent), this);

			_inputProvider = new UserInputProvider();
			StackEngine = new StackEngine(_game, services, _inputProvider, GameSettings);

			if (GameSettings.Debug)
			{
				_inputProvider.Handler += HandleDebugInputEvent;
			}

			_inputProvider.Handler += HandleSkipInputEvent;

			StackEngine.OnExit += Exit;

			_counter = new FrameRateCounter();
			SetSpeed(GameSpeed.Default);

			base.Initialize();
		}

		protected override void UnloadContent()
		{
			StackEngine.Dispose();
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
				_counter.UpdateStart();
			}

			StackEngine.Update();

			if (EngineVariables.ShowFPS)
			{
				_counter.UpdateEnd();
			}

			if (SkipCutscene.Enabled)
			{
				SuppressDraw();
			}

			base.Update(time);
		}

		protected override void Draw(GameTime time)
		{
			StackEngine.Draw();

			if (EngineVariables.ShowFPS)
			{
				_counter.Draw(StackEngine.Renderer.SpriteBatch, StackEngine.Renderer.DefaultFont, Vector2.Zero);
			}
		}

		public void SetSpeed(GameSpeed speed)
		{
			if (!_gameSpeed.Equals(speed))
			{
				Log.WriteLine("Setting game speed to " + speed.Description);
				_gameSpeed = speed;
				TargetElapsedTime = speed.TargetElapsedTime;
			}
		}
	}
}
