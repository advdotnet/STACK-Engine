using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using STACK.Components;
using STACK.TestBase;
using System;
using System.Collections.Generic;

namespace STACK.Functional.Test
{
	[TestClass]
	public class Input
	{

		public class TestGame : StackGame
		{
			private class TestScene : Scene
			{
				public TestScene()
				{
					InputDispatcher
						.Create(this)
						.SetOnMouseUpFn(OnMouseUp);
				}

				public void OnMouseUp(Vector2 position, MouseButton button)
				{
					throw new Exception();
				}
			}

			protected override List<Scene> GetScenes() => new List<Scene> { new TestScene() { Enabled = true } };

			protected override void OnStart()
			{
				StartWorld();
			}
		}

		[TestMethod, TestCategory("GPU")]
		public void InputNotSendToWorldWhenPaused()
		{
			using (var graphicsDevice = Mock.CreateGraphicsDevice())
			using (var runner = new TestEngine(new TestGame(), Mock.Wrap(graphicsDevice), Mock.Input))
			{
				runner.Pause();
				runner.MouseClick(10, 10);
			}
		}
	}
}
