using Microsoft.VisualStudio.TestTools.UnitTesting;
using STACK.Components;

namespace STACK.Functional.Test
{
    [TestClass]
    public class Engine
    {
        [TestMethod, TestCategory("GPU")]
        public void EngineLoadsWorldComponentStates()
        {
            using (var GraphicsDevice = Mock.CreateGraphicsDevice())
            using (var Engine = new StackEngine(StackGame.Empty, Mock.Wrap(GraphicsDevice), Mock.Input, GameSettings.LoadFromConfigFile("")))
            {
                var Bytes1 = State.Serialization.SaveState(Engine.Game.World);
                var SaveGame1 = new STACK.SaveGame("TestSave", Bytes1, null);

                Engine.Game.World.Get<Camera>().Zoom = 2f;

                var Bytes2 = State.Serialization.SaveState(Engine.Game.World);
                var SaveGame2 = new STACK.SaveGame("TestSave", Bytes2, null);

                Engine.LoadState(SaveGame1);
                Assert.AreEqual(1f, Engine.Game.World.Get<Camera>().Zoom);
                Engine.LoadState(SaveGame2);
                Assert.AreEqual(2f, Engine.Game.World.Get<Camera>().Zoom);
            }
        }
    }
}
