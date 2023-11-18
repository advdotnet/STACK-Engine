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

			private class Player : Entity
			{
				public Player()
				{
					Transform
						.Create(this);

					CameraLocked
						.Create(this);
				}
			}

			private class TestScene : Location
			{
				public TestScene() : base("stacklogo")
				{
					Enabled = true;
					Push(new Player());
				}

			}

			protected override List<Scene> GetScenes()
			{
				return new List<Scene> { new TestScene() };
			}

			protected override void OnStart()
			{
				StartWorld();
			}
		}

		[TestMethod, TestCategory("GPU")]
		public void PerformanceTestCameraLocked()
		{
			using (var graphicsDevice = Mock.CreateGraphicsDevice())
			using (var runner = new TestEngine(new PerformanceTestGame(), Mock.Wrap(graphicsDevice), Mock.Input))
			{
				var player = runner.Game.World.GetGameObject("STACK.Functional.Test.Performance+PerformanceTestGame+Player");
				var playerTransform = player.Get<Transform>();
				var background = ((Location)runner.Game.World.Scenes[0]).Background.Get<Sprite>();
				var backgroundWidth = background.GetWidth();
				var watch = new Stopwatch();
				var right = true;
				runner.Resume();
				playerTransform.SetPosition(0, 0);

				watch.Start();
				for (var i = 0; i < 100000; i++)
				{
					playerTransform.SetPosition(playerTransform.Position.X + (right ? 10 : -10), playerTransform.Position.Y);

					if (playerTransform.Position.X >= backgroundWidth)
					{
						right = false;
					}
					else if (playerTransform.Position.X <= 0)
					{
						right = true;
					}

					runner.Tick();
				}
				watch.Stop();
				System.Console.WriteLine("Elapsed: " + watch.ElapsedMilliseconds);
			}
		}
	}
}
