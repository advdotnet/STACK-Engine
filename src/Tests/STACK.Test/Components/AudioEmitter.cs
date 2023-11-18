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
			var entity = new Entity();
			var audioEmitterComponent = AudioEmitter.Create(entity).SetUpdatePositionWithTransform(true);
			_ = Transform.Create(entity).SetPosition(1, 2);

			entity.Initialize(false);

			Assert.AreEqual(1 / audioEmitterComponent.Scale, audioEmitterComponent.Emitter.Position.X);
			Assert.AreEqual(2 / audioEmitterComponent.Scale, audioEmitterComponent.Emitter.Position.Y);
		}

		[TestMethod]
		public void EmitterUpdatesPositionWithTransformTest()
		{
			var entity = new Entity();
			var audioEmitterComponent = AudioEmitter.Create(entity).SetUpdatePositionWithTransform(true);
			var transformComponent = Transform.Create(entity);

			entity.Initialize(false);

			transformComponent.Position = new Microsoft.Xna.Framework.Vector2(1, 2);

			Assert.AreEqual(1 / audioEmitterComponent.Scale, audioEmitterComponent.Emitter.Position.X);
			Assert.AreEqual(2 / audioEmitterComponent.Scale, audioEmitterComponent.Emitter.Position.Y);
		}

		[TestMethod]
		public void ListenerUpdatesPositionWithTransformInitiallyTest()
		{
			var entity = new Entity();
			var audioListenerComponent = AudioListener.Create(entity).SetUpdatePositionWithTransform(true);
			_ = Transform.Create(entity).SetPosition(1, 2);

			entity.Initialize(false);

			Assert.AreEqual(1 / audioListenerComponent.Scale, audioListenerComponent.Listener.Position.X);
			Assert.AreEqual(2 / audioListenerComponent.Scale, audioListenerComponent.Listener.Position.Y);
		}

		[TestMethod]
		public void ListenerUpdatesPositionWithTransformTest()
		{
			var entity = new Entity();
			var audioListenerComponent = AudioListener.Create(entity).SetUpdatePositionWithTransform(true);
			var transformComponent = Transform.Create(entity);

			entity.Initialize(false);

			transformComponent.Position = new Microsoft.Xna.Framework.Vector2(1, 2);

			Assert.AreEqual(1 / audioListenerComponent.Scale, audioListenerComponent.Listener.Position.X);
			Assert.AreEqual(2 / audioListenerComponent.Scale, audioListenerComponent.Listener.Position.Y);
		}
	}
}
