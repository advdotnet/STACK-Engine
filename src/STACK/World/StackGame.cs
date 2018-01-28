using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace STACK
{
    /// <summary>
    /// Class used to provide a list of scenes.
    /// </summary>
    public abstract class StackGame
    {
        public Point VirtualResolution { get; protected set; }
        public string Title { get; protected set; }
        public string SaveGameFolder { get; protected set; }
        public World World;
        public static StackEngine Engine;

        /// <summary>
        /// Scenes are added here.
        /// </summary>
        protected abstract List<Scene> GetScenes();

        public StackGame()
        {
            VirtualResolution = new Point(640, 400);
            SaveGameFolder = "STACK";
            Title = "STACK Game";
        }

        public void Start(StackEngine engine)
        {
            Engine = engine;
            OnStart();
        }

        protected virtual void OnStart() { }
        protected virtual void OnRestore() { }
        protected virtual void HandleInputEvent() { }
        protected virtual void OnWorldStart() { }
        protected virtual void OnWorldInitialized() { }

        public void RestoreState(SaveGame state)
        {
            var DeserializedWorld = State.State.LoadState<World>(state.World);

            if (World != null)
            {
                if (World.Loaded)
                {
                    World.UnloadContent();
                }

                World.Unsubscribe(Engine.InputProvider);
            }

            World = new World(Engine.Services, Engine.InputProvider, VirtualResolution, GetScenes());
            World.Initialize();
            World._Scenes = null;
            World.RestoreState(DeserializedWorld, Engine.Services, Engine.GetWorldContent());

            OnRestore();
        }

        public void StartWorld()
        {
            if (World != null)
            {
                if (World.Loaded)
                {
                    World.UnloadContent();
                }

                World.Unsubscribe(Engine.InputProvider);
            }

            World = new World(Engine.Services, Engine.InputProvider, VirtualResolution, GetScenes());
            World._Scenes = null;
            World.Initialize();

            OnWorldInitialized();

            World.LoadContent(Engine.GetWorldContent());

            OnWorldStart();
        }

        public static EmptyGame Empty
        {
            get
            {
                return new EmptyGame();
            }
        }
    }

    public class EmptyGame : StackGame
    {
        protected override List<Scene> GetScenes()
        {
            return new List<Scene> { new Scene("1") };
        }

        protected override void OnStart()
        {
            StartWorld();
        }
    }
}
