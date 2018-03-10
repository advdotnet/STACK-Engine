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
        public Renderer Renderer { get; private set; }
        public StackGame Game { get; private set; }
        public ContentLoader EngineContent { get; private set; }
        public IServiceProvider Services { get; private set; }
        public Console Console { get; private set; }
        public InputProvider InputProvider { get; private set; }
        public event Action OnExit;
        ContentLoader WorldContent { get; set; }

        public StackEngine(StackGame game, IServiceProvider services, InputProvider input, GameSettings gameSettings)
        {
            Services = services;
            gameSettings.SetCulture();

            var ConsoleLogHandler = new ConsoleLogHandler(Console);
            EngineContent = new ContentLoader(Services);
            Renderer = new Renderer(Services, EngineContent, game.VirtualResolution, gameSettings.GetTargetResolution(game.VirtualResolution), gameSettings.Bloom);

            Game = game;

            Console = new Console(this);
            ConsoleLogHandler.Console = Console;
            Paused = true;

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
            if (null != WorldContent)
            {
                WorldContent.Dispose();
            }

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

        public bool Paused { get; private set; }

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
            Dispose();

            OnExit?.Invoke();
        }

        public void SaveState(string name = "game1")
        {
            var ScreenshotData = Renderer.GetScreenshotPNGData(Game.World);
            SaveGame.SaveToFile(Game.SaveGameFolder, name, Game.World, ScreenshotData);
        }

        public void LoadState(SaveGame state)
        {
            Game.RestoreState(state);
        }

        public void LoadState(string name = "game1")
        {
            var State = SaveGame.LoadFromFile(Game.SaveGameFolder, name);
            LoadState(State);
        }

        public Dictionary<string, SaveGame> GetSaveGames()
        {
            return SaveGame.GetSaveGames(Game.SaveGameFolder);
        }
    }
}
