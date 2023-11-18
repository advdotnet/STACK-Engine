using Microsoft.VisualStudio.TestTools.UnitTesting;
using STACK.Components;
using STACK.TestBase;

namespace STACK.Functional.Test
{
	[TestClass]
	public class Engine
	{
		[TestMethod, TestCategory("GPU")]
		public void EngineLoadsWorldComponentStates()
		{
			using (var graphicsDevice = Mock.CreateGraphicsDevice())
			using (var engine = new StackEngine(StackGame.Empty, Mock.Wrap(graphicsDevice), Mock.Input, GameSettings.LoadFromConfigFile("")))
			{
				var bytes1 = State.Serialization.SaveState(engine.Game.World);
				var saveGame1 = new STACK.SaveGame("TestSave", bytes1, null);

				engine.Game.World.Get<Camera>().Zoom = 2f;

				var bytes2 = State.Serialization.SaveState(engine.Game.World);
				var saveGame2 = new STACK.SaveGame("TestSave", bytes2, null);

				engine.LoadState(saveGame1);
				Assert.AreEqual(1f, engine.Game.World.Get<Camera>().Zoom);
				engine.LoadState(saveGame2);
				Assert.AreEqual(2f, engine.Game.World.Get<Camera>().Zoom);
			}
		}

		[TestMethod, TestCategory("GPU")]
		public void GraphicsDeviceMockAvaiable()
		{
			using (var graphicsDevice = Mock.CreateGraphicsDevice())
			{
				Assert.IsNotNull(graphicsDevice.GraphicsDevice);
			}
		}
	}
}
