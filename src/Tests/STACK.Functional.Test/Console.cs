using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework.Input;

namespace STACK.Functional.Test
{
    [TestClass]
    public class Console
    {        
        [TestMethod, TestCategory("GPU")]
        public void ConsoleToggle()
        {
			using (var GraphicsDevice = Mock.CreateGraphicsDevice())
			using (var Runner = new TestEngine(StackGame.Empty, Mock.Wrap(GraphicsDevice), Mock.Input))
			{
				Runner.Console.Visible = false;
				Runner.Console.Toggle();
				Assert.IsTrue(Runner.Console.Visible);
				Runner.Console.Toggle();
				Assert.IsFalse(Runner.Console.Visible);
			}
        }

        [TestMethod, TestCategory("GPU")]
        public void ConsoleInput()
        {
			using (var GraphicsDevice = Mock.CreateGraphicsDevice())
			using (var Runner = new TestEngine(StackGame.Empty, Mock.Wrap(GraphicsDevice), Mock.Input))
			{
				Runner.Console.Visible = true;
				Runner.KeyPress(Keys.A);
				Runner.KeyPress(Keys.LeftShift, Keys.A);
				Assert.AreEqual("aA", Runner.Console.Input);
			}
        }

        [TestMethod, TestCategory("GPU")]
        public void SetCommand()
        {
			using (var GraphicsDevice = Mock.CreateGraphicsDevice())
			using (var Runner = new TestEngine(StackGame.Empty, Mock.Wrap(GraphicsDevice), Mock.Input))
			{
				EngineVariables.DebugPath = false;
				Runner.Console.Visible = true;
				Runner.EnterText("set stack.debugpath = true");
				Assert.IsTrue(EngineVariables.DebugPath);				
			}
        }

        [TestMethod, TestCategory("GPU")]        
        public void ExitCommand()
        {
			using (var GraphicsDevice = Mock.CreateGraphicsDevice())
			using (var Runner = new TestEngine(StackGame.Empty, Mock.Wrap(GraphicsDevice), Mock.Input))
			{
				bool ExitCalled = false;
				Runner.OnExit += () => { ExitCalled = true; };

				Runner.Console.Visible = true;
				Runner.EnterText("exit");

				Assert.IsTrue(ExitCalled);			
			}
        }      
    }
}
