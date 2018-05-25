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
        ContentLoader WorldContent { get; set; }
        bool _Paused = true;

        public StackEngine(StackGame game, IServiceProvider services, InputProvider input, GameSettings gameSettings)
        {
            GameSettings = gameSettings;
            Services = services;
            gameSettings.SetCulture();

            var ConsoleLogHandler = new ConsoleLogHandler(Console);
            EngineContent = new ContentLoader(Services);
            Renderer = new Renderer(Services, EngineContent, game.VirtualResolution, gameSettings.GetTargetResolution(game.VirtualResolution), gameSettings.Bloom);

            Game = game;

            Console = new Console(this);
            ConsoleLogHandler.Console = Console;
            _Paused = true;

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

        public bool Paused
        {
            get
            {
                return _Paused;
            }
            private set
            {
                if (value == _Paused)
                {
                    return;
                }

                _Paused = value;
                if (_Paused)
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
            if (null != Game)
            {
                Game.OnExit();
            }

            Dispose();

            OnExit?.Invoke();
        }

        public void ApplyGameSettingsVolume()
        {
            if (null != Game.World && null != GameSettings)
            {
                Game.World.Get<AudioManager>().SoundEffectVolume = GameSettings.SoundEffectVolume;
                Game.World.Get<AudioManager>().MusicVolume = GameSettings.MusicVolume;
            }
        }

        public SaveGame SaveState(string name = "game1")
        {
            var ScreenshotData = Renderer.GetScreenshotPNGData(Game.World);

            return SaveGame.SaveToFile(Game.SaveGameFolder, name, Game.World, ScreenshotData);
        }

        public void LoadState(SaveGame state)
        {
            Game.RestoreState(state);
        }

        public void LoadState(string fileName = "game1")
        {
            var State = SaveGame.LoadFromFile(Game.SaveGameFolder, fileName);
            LoadState(State);
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
