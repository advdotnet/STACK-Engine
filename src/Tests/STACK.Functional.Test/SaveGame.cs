using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using STACK.Input;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace STACK.Functional.Test
{
    [TestClass]
    public class SaveGame
    {

        public class Green : Entity
        {
            public override void OnDraw(Graphics.Renderer renderer)
            {
                renderer.PrimitivesRenderer.DrawRectangle(new Microsoft.Xna.Framework.Rectangle(0, 0, 1280, 720), Microsoft.Xna.Framework.Color.Green);
            }
        }

        [TestMethod]
        public void ScreenshotGreen()
        {
            using (var GraphicsDevice = Mock.CreateGraphicsDevice())
            using (var Runner = new TestEngine(StackGame.Empty, Mock.Wrap(GraphicsDevice), Mock.Input))
            {
                Runner.StartGame();
                var Scene = Runner.Game.World.Scenes.FirstOrDefault();
                Scene.Push(new Green());
                Scene.Visible = true;
                var PNGData = Runner.Renderer.GetScreenshotPNGData(Runner.Game.World);

                using (var ScreenshotStream = new MemoryStream(PNGData))
                {
                    using (var Screenshot = Texture2D.FromStream(Runner.Renderer.GraphicsDevice, ScreenshotStream))
                    {
                        Color[] Colors = new Color[Screenshot.Width * Screenshot.Height];
                        Screenshot.GetData(Colors);
                        for (int i = 0; i < Colors.Length; i++)
                        {
                            Assert.AreEqual(Color.Green, Colors[i]);
                        }
                    }
                }
            }
        }

        private int GetSubscriberCount(InputProvider input)
        {
            var fieldInfo = typeof(InputProvider).GetField("Handler", BindingFlags.NonPublic | BindingFlags.Instance);
            var field = fieldInfo.GetValue(input);
            var eventDelegate = (MulticastDelegate)field;

            return eventDelegate.GetInvocationList().Count();
        }

        [TestMethod]
        public void RestartGameOneInput()
        {
            using (var GraphicsDevice = Mock.CreateGraphicsDevice())
            using (var Runner = new TestEngine(StackGame.Empty, Mock.Wrap(GraphicsDevice), Mock.Input))
            {
                Runner.StartGame();
                var Subscribercount = GetSubscriberCount(Runner.InputProvider);
                Assert.AreEqual(1, Subscribercount);

                // Restart Game
                Runner.StartGame();
                Subscribercount = GetSubscriberCount(Runner.InputProvider);

                Assert.AreEqual(1, Subscribercount);
            }
        }

        [TestMethod]
        public void LoadSaveGameAfterWorldStart()
        {
            STACK.SaveGame State;

            using (var GraphicsDevice = Mock.CreateGraphicsDevice())
            using (var Runner = new TestEngine(StackGame.Empty, Mock.Wrap(GraphicsDevice), Mock.Input))
            {
                Runner.StartGame();
                Runner.Game.World.Scenes.FirstOrDefault().Push(new Entity("newobj"));
                Runner.Game.World.Interactive = false;
                State = new STACK.SaveGame("utest", STACK.State.State.SaveState<World>(Runner.Game.World), new byte[0] { });
            }

            using (var GraphicsDevice = Mock.CreateGraphicsDevice())
            using (var Runner = new TestEngine(StackGame.Empty, Mock.Wrap(GraphicsDevice), Mock.Input))
            {
                Runner.StartGame();
                Runner.LoadState(State);
                Assert.AreEqual(1, Runner.Game.World.Scenes.FirstOrDefault().Entities.Count);
                Assert.AreEqual("newobj", Runner.Game.World.Scenes.FirstOrDefault().Entities.First().ID);
                Assert.IsFalse(Runner.Game.World.Interactive);
            }
        }

        [TestMethod]
        public void LoadSaveGameBeforeWorldStart()
        {
            STACK.SaveGame State;

            using (var GraphicsDevice = Mock.CreateGraphicsDevice())
            using (var Runner = new TestEngine(StackGame.Empty, Mock.Wrap(GraphicsDevice), Mock.Input))
            {
                Runner.StartGame();
                Runner.Game.World.Scenes.FirstOrDefault().Push(new Entity("newobj"));
                Runner.Game.World.Interactive = false;
                State = new STACK.SaveGame("utest", STACK.State.State.SaveState<World>(Runner.Game.World), new byte[0] { });
            }

            using (var GraphicsDevice = Mock.CreateGraphicsDevice())
            using (var Runner = new TestEngine(StackGame.Empty, Mock.Wrap(GraphicsDevice), Mock.Input))
            {
                Runner.LoadState(State);
                Assert.AreEqual(1, Runner.Game.World.Scenes.FirstOrDefault().Entities.Count);
                Assert.AreEqual("newobj", Runner.Game.World.Scenes.FirstOrDefault().Entities.First().ID);
                Assert.IsFalse(Runner.Game.World.Interactive);
            }
        }



    }
}
