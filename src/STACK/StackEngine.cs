using Microsoft.Xna.Framework.Media;
using STACK.Components;
using STACK.Input;
using STACK.Logging;
using System;
using System.Collections.Generic;
using Console = STACK.Debug.Console;
using Renderer = STACK.Graphics.Renderer;

namespace STACK
{

	public class StackEngine : IDisposable
	{
		public GameSettings GameSettings { get; private set; }
		public Renderer Renderer { get; private set; }
		public StackGame Game { get; private set; }
		public ContentLoader EngineContent { get; private set; }
		public IServiceProvider Services { get; private set; }
		public Console Console { get; private set; }
		public InputProvider InputProvider { get; private set; }
		public event Action OnExit;

		private ContentLoader WorldContent { get; set; }

		private bool _paused = true;

		public StackEngine(StackGame game, IServiceProvider services, InputProvider input, GameSettings gameSettings)
		{
			GameSettings = gameSettings;
			Services = services;
			gameSettings.SetCulture();

			var consoleLogHandler = new ConsoleLogHandler(Console);
			EngineContent = new ContentLoader(Services);
			Renderer = new Renderer(Services, EngineContent, game.VirtualResolution, gameSettings.GetTargetResolution(game.VirtualResolution), gameSettings.Bloom);

			Game = game;

			Console = new Console(this);
			consoleLogHandler.Console = Console;
			_paused = true;

			InputProvider = input;

			if (InputProvider != null)
			{
				InputProvider.DisplaySettings = Renderer.DisplaySettings;
			}

			EngineVariables.EnableGUI = true;
			Game.Start(this);
		}

		public ContentLoader GetWorldContent()
		{
			WorldContent?.Dispose();

			WorldContent = new ContentLoader(Services);

			return WorldContent;
		}

		protected StackEngine() { }

		public void Draw()
		{
			Renderer.Draw(Game?.World);
		}

		public void Update()
		{
			InputProvider.Dispatch(Paused);

			Renderer.Update(InputProvider.KeyboardState, InputProvider.MouseState);

			if (Game != null && Game.World != null && !Paused)
			{
				Game.World.Update();
			}
		}

		public void Dispose()
		{
			Game.World?.UnloadContent();
			Renderer.Dispose();
			WorldContent?.Dispose();
			EngineContent.Dispose();
		}

		public void StartGame()
		{
			Game.StartWorld();
			Resume();
		}

		public bool Paused
		{
			get => _paused;
			private set
			{
				if (value == _paused)
				{
					return;
				}

				_paused = value;

				if (null != Game.World)
				{
					Game.World.Get<AudioManager>().IsEnginePaused = value;
				}

				if (_paused)
				{
					MediaPlayer.Pause();
				}
				else
				{
					MediaPlayer.Resume();
				}
			}
		}

		public void Pause(bool paused = true)
		{
			Paused = paused;
		}

		public void Resume()
		{
			Paused = false;
		}

		public void Exit()
		{
			Game?.OnExit();

			Dispose();

			OnExit?.Invoke();
		}

		public void ApplyGameSettingsVolume()
		{
			Game.World?.Get<AudioManager>().ApplyGameSettingsVolume(GameSettings);
		}

		public SaveGame SaveState(string name = "game1")
		{
			var screenshotData = Renderer.GetScreenshotPNGData(Game.World);

			return SaveGame.SaveToFile(Game.SaveGameFolder, name, Game.World, screenshotData);
		}

		public void LoadState(SaveGame state)
		{
			Game.RestoreState(state);
		}

		public void LoadState(string fileName = "game1")
		{
			var state = SaveGame.LoadFromFile(Game.SaveGameFolder, fileName);
			LoadState(state);
		}

		public string ExistsStateByName(string name)
		{
			return SaveGame.ExistsStateByName(Game.SaveGameFolder, name);
		}

		public Dictionary<string, SaveGame> GetSaveGames()
		{
			return SaveGame.GetSaveGames(Game.SaveGameFolder);
		}
	}
}
