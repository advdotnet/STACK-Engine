using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using STACK.Input;
using STACK.TestBase;
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
				renderer.PrimitivesRenderer.DrawRectangle(new Rectangle(0, 0, 1280, 720), Color.Green);
			}
		}

		[TestMethod, TestCategory("GPU")]
		public void ScreenshotGreen()
		{
			using (var graphicsDevice = Mock.CreateGraphicsDevice())
			using (var runner = new TestEngine(StackGame.Empty, Mock.Wrap(graphicsDevice), Mock.Input))
			{
				runner.StartGame();
				var scene = runner.Game.World.Scenes.FirstOrDefault();
				scene.Push(new Green());
				scene.Visible = true;
				var pngData = runner.Renderer.GetScreenshotPNGData(runner.Game.World);

				using (var screenshotStream = new MemoryStream(pngData))
				{
					using (var screenshot = Texture2D.FromStream(runner.Renderer.GraphicsDevice, screenshotStream))
					{
						var colors = new Color[screenshot.Width * screenshot.Height];
						screenshot.GetData(colors);
						for (var i = 0; i < colors.Length; i++)
						{
							Assert.AreEqual(Color.Green, colors[i]);
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

		[TestMethod, TestCategory("GPU")]
		public void RestartGameOneInput()
		{
			using (var graphicsDevice = Mock.CreateGraphicsDevice())
			using (var runner = new TestEngine(StackGame.Empty, Mock.Wrap(graphicsDevice), Mock.Input))
			{
				runner.StartGame();
				var subscribercount = GetSubscriberCount(runner.InputProvider);
				Assert.AreEqual(1, subscribercount);

				// Restart Game
				runner.StartGame();
				subscribercount = GetSubscriberCount(runner.InputProvider);

				Assert.AreEqual(1, subscribercount);
			}
		}

		[TestMethod, TestCategory("GPU")]
		public void LoadSaveGameAfterWorldStart()
		{
			STACK.SaveGame state;

			using (var graphicsDevice = Mock.CreateGraphicsDevice())
			using (var runner = new TestEngine(StackGame.Empty, Mock.Wrap(graphicsDevice), Mock.Input))
			{
				runner.StartGame();
				runner.Game.World.Scenes.FirstOrDefault().Push(new Entity("newobj"));
				runner.Game.World.Interactive = false;
				state = new STACK.SaveGame("utest", STACK.State.Serialization.SaveState(runner.Game.World), new byte[0] { });
			}

			using (var graphicsDevice = Mock.CreateGraphicsDevice())
			using (var runner = new TestEngine(StackGame.Empty, Mock.Wrap(graphicsDevice), Mock.Input))
			{
				runner.StartGame();
				runner.LoadState(state);
				Assert.AreEqual(1, runner.Game.World.Scenes.FirstOrDefault().GameObjectCache.Entities.Count);
				Assert.AreEqual("newobj", runner.Game.World.Scenes.FirstOrDefault().GameObjectCache.Entities.First().ID);
				Assert.IsFalse(runner.Game.World.Interactive);
			}
		}

		[TestMethod, TestCategory("GPU")]
		public void LoadSaveGameBeforeWorldStart()
		{
			STACK.SaveGame state;

			using (var graphicsDevice = Mock.CreateGraphicsDevice())
			using (var runner = new TestEngine(StackGame.Empty, Mock.Wrap(graphicsDevice), Mock.Input))
			{
				runner.StartGame();
				runner.Game.World.Scenes.FirstOrDefault().Push(new Entity("newobj"));
				runner.Game.World.Interactive = false;
				state = new STACK.SaveGame("utest", STACK.State.Serialization.SaveState(runner.Game.World), new byte[0] { });
			}

			using (var graphicsDevice = Mock.CreateGraphicsDevice())
			using (var runner = new TestEngine(StackGame.Empty, Mock.Wrap(graphicsDevice), Mock.Input))
			{
				runner.LoadState(state);
				Assert.AreEqual(1, runner.Game.World.Scenes.FirstOrDefault().GameObjectCache.Entities.Count);
				Assert.AreEqual("newobj", runner.Game.World.Scenes.FirstOrDefault().GameObjectCache.Entities.First().ID);
				Assert.IsFalse(runner.Game.World.Interactive);
			}
		}
	}
}
