using Microsoft.Xna.Framework;
using STACK;
using System.Collections.Generic;

namespace Actor
{
    /// <summary>
    /// The game class
    /// </summary>
    public class ActorGame : StackGame
    {
        public const int VIRTUAL_WIDTH = 640;
        public const int VIRTUAL_HEIGHT = 400;

        public ActorGame()
        {
            VirtualResolution = new Point(VIRTUAL_WIDTH, VIRTUAL_HEIGHT);
            Title = "Playground: Actor";
        }

        public static Ego Ego
        {
            get
            {
                return (Ego)Engine.Game.World.GetScene("Actor.Scene").GetObject("Actor.Ego");
            }
        }

        public static ShadowEgo ShadowEgo
        {
            get
            {
                return (ShadowEgo)Engine.Game.World.GetScene("Actor.Scene").GetObject("Actor.ShadowEgo");
            }
        }

        protected override List<STACK.Scene> GetScenes()
        {
            return new List<STACK.Scene>()
            {
                new Scene(),
            };
        }

        protected override void OnStart()
        {
            this.StartWorld();
            Engine.Resume();
        }
    }
}
