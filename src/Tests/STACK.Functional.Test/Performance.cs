using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using STACK.Components;
using STACK.TestBase;
using System.Collections.Generic;
using System.Diagnostics;

namespace STACK.Functional.Test
{
    [TestClass]
    public class Performance
    {

        public class PerformanceTestGame : StackGame
        {
            public PerformanceTestGame()
            {
                VirtualResolution = new Point(30, 20);
            }

            class Player : Entity
            {
                public Player()
                {
                    Transform
                        .Create(this);

                    CameraLocked
                        .Create(this);
                }
            }

            class TestScene : Location
            {
                public TestScene() : base("stacklogo")
                {
                    Enabled = true;
                    this.Push(new Player());
                }

            }

            protected override List<Scene> GetScenes()
            {
                return new List<Scene> { new TestScene() };
            }

            protected override void OnStart()
            {
                this.StartWorld();
            }
        }

        [TestMethod, TestCategory("GPU")]
        public void PerformanceTestCameraLocked()
        {
            using (var GraphicsDevice = Mock.CreateGraphicsDevice())
            using (var Runner = new TestEngine(new PerformanceTestGame(), Mock.Wrap(GraphicsDevice), Mock.Input))
            {
                var Player = Runner.Game.World.GetGameObject("STACK.Functional.Test.Performance+PerformanceTestGame+Player");
                var PlayerTransform = Player.Get<Transform>();
                var Background = ((Location)Runner.Game.World.Scenes[0]).Background.Get<Sprite>();
                var BackgroundWidth = Background.GetWidth();
                var Watch = new Stopwatch();
                var Right = true;
                Runner.Resume();
                PlayerTransform.SetPosition(0, 0);

                Watch.Start();
                for (int i = 0; i < 100000; i++)
                {
                    PlayerTransform.SetPosition(PlayerTransform.Position.X + (Right ? 10 : -10), PlayerTransform.Position.Y);

                    if (PlayerTransform.Position.X >= BackgroundWidth)
                    {
                        Right = false;
                    }
                    else if (PlayerTransform.Position.X <= 0)
                    {
                        Right = true;
                    }

                    Runner.Tick();
                }
                Watch.Stop();
                System.Console.WriteLine("Elapsed: " + Watch.ElapsedMilliseconds);
            }
        }
    }
}
