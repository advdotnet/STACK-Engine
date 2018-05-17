using Microsoft.VisualStudio.TestTools.UnitTesting;
using STACK.Components;

namespace STACK.Test
{
    [TestClass]
    public class AudioTest
    {
        [TestMethod]
        public void EmitterUpdatesPositionWithTransformInitiallyTest()
        {
            var Entity = new Entity();
            var AudioEmitterComponent = AudioEmitter.Create(Entity).SetUpdatePositionWithTransform(true);
            var TransformComponent = Transform.Create(Entity).SetPosition(1, 2);

            Entity.Initialize(false);

            Assert.AreEqual(1 / AudioEmitterComponent.Scale, AudioEmitterComponent.Emitter.Position.X);
            Assert.AreEqual(2 / AudioEmitterComponent.Scale, AudioEmitterComponent.Emitter.Position.Y);
        }

        [TestMethod]
        public void EmitterUpdatesPositionWithTransformTest()
        {
            var Entity = new Entity();
            var AudioEmitterComponent = AudioEmitter.Create(Entity).SetUpdatePositionWithTransform(true);
            var TransformComponent = Transform.Create(Entity);

            Entity.Initialize(false);

            TransformComponent.Position = new Microsoft.Xna.Framework.Vector2(1, 2);

            Assert.AreEqual(1 / AudioEmitterComponent.Scale, AudioEmitterComponent.Emitter.Position.X);
            Assert.AreEqual(2 / AudioEmitterComponent.Scale, AudioEmitterComponent.Emitter.Position.Y);
        }

        [TestMethod]
        public void ListenerUpdatesPositionWithTransformInitiallyTest()
        {
            var Entity = new Entity();
            var AudioListenerComponent = AudioListener.Create(Entity).SetUpdatePositionWithTransform(true);
            var TransformComponent = Transform.Create(Entity).SetPosition(1, 2);

            Entity.Initialize(false);

            Assert.AreEqual(1 / AudioListenerComponent.Scale, AudioListenerComponent.Listener.Position.X);
            Assert.AreEqual(2 / AudioListenerComponent.Scale, AudioListenerComponent.Listener.Position.Y);
        }

        [TestMethod]
        public void ListenerUpdatesPositionWithTransformTest()
        {
            var Entity = new Entity();
            var AudioListenerComponent = AudioListener.Create(Entity).SetUpdatePositionWithTransform(true);
            var TransformComponent = Transform.Create(Entity);

            Entity.Initialize(false);

            TransformComponent.Position = new Microsoft.Xna.Framework.Vector2(1, 2);

            Assert.AreEqual(1 / AudioListenerComponent.Scale, AudioListenerComponent.Listener.Position.X);
            Assert.AreEqual(2 / AudioListenerComponent.Scale, AudioListenerComponent.Listener.Position.Y);
        }
    }
}
