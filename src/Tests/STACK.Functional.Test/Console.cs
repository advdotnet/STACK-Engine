using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework.Input;
using STACK.TestBase;

namespace STACK.Functional.Test
{
	[TestClass]
	public class Console
	{
		[TestMethod, TestCategory("GPU")]
		public void ConsoleToggle()
		{
			using (var graphicsDevice = Mock.CreateGraphicsDevice())
			using (var runner = new TestEngine(StackGame.Empty, Mock.Wrap(graphicsDevice), Mock.Input))
			{
				runner.Console.Visible = false;
				runner.Console.Toggle();
				Assert.IsTrue(runner.Console.Visible);
				runner.Console.Toggle();
				Assert.IsFalse(runner.Console.Visible);
			}
		}

		[TestMethod, TestCategory("GPU")]
		public void ConsoleInput()
		{
			using (var graphicsDevice = Mock.CreateGraphicsDevice())
			using (var runner = new TestEngine(StackGame.Empty, Mock.Wrap(graphicsDevice), Mock.Input))
			{
				runner.Console.Visible = true;
				runner.KeyPress(Keys.A);
				runner.KeyPress(Keys.LeftShift, Keys.A);
				Assert.AreEqual("aA", runner.Console.Input);
			}
		}

		[TestMethod, TestCategory("GPU")]
		public void SetCommand()
		{
			var gameSettings = new GameSettings()
			{
				Culture = "de-DE"
			};

			using (var graphicsDevice = Mock.CreateGraphicsDevice())
			using (var runner = new TestEngine(StackGame.Empty, Mock.Wrap(graphicsDevice), Mock.Input, gameSettings))
			{
				EngineVariables.DebugPath = false;
				runner.Console.Visible = true;
				runner.EnterText("set stack.debugpath = true");
				Assert.IsTrue(EngineVariables.DebugPath);
			}
		}

		[TestMethod, TestCategory("GPU")]
		public void ExitCommand()
		{
			using (var graphicsDevice = Mock.CreateGraphicsDevice())
			using (var runner = new TestEngine(StackGame.Empty, Mock.Wrap(graphicsDevice), Mock.Input))
			{
				var exitCalled = false;
				runner.OnExit += () => exitCalled = true;

				runner.Console.Visible = true;
				runner.EnterText("exit");

				Assert.IsTrue(exitCalled);
			}
		}
	}
}
