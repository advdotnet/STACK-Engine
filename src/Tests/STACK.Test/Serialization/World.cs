using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using STACK.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace STACK.Test
{
	[TestClass]
	public class WorldState
	{
		[TestMethod]
		public void SerializeGameObject()
		{
			var test = new Entity("stackobj");
			test.Add<Transform>().Position = Vector2.UnitY;

			var check = State.Serialization.SaveState(test);

			var second = State.Serialization.LoadState<Entity>(check);
			Assert.AreEqual(test.ID, second.ID);
			Assert.AreEqual(test.Items.Count, second.Items.Count);
			Assert.AreEqual(test.Get<Transform>().Position, second.Get<Transform>().Position);
		}

		[TestMethod]
		public void TestSize()
		{
			var test = new Entity("obj");
			System.Diagnostics.Debug.WriteLine(State.Serialization.SaveState(test).Length);
			var stack = new Scene("stack");
			System.Diagnostics.Debug.WriteLine(State.Serialization.SaveState(stack).Length);
			stack.Push(test);
			System.Diagnostics.Debug.WriteLine(State.Serialization.SaveState(stack).Length);
		}

		[Serializable]
		private class Derived : Entity
		{
			public List<string> List = new List<string>();
			public Derived(string id) : base(id) { }
		}

		[TestMethod]
		public void DerivedGameObjectState()
		{
			var test = new Derived("derived");

			test.List.Add("list-item");
			var check = State.Serialization.SaveState<Entity>(test);
			var second = (Derived)State.Serialization.LoadState<Entity>(check);
			Assert.AreEqual(test.ID, second.ID);
			Assert.AreEqual(1, second.List.Count);
			Assert.AreEqual(test.List[0], second.List[0]);
		}

		[TestMethod]
		public void ReferencesAfterDeserializing()
		{
			var world = WorldTest.GetTestWorld();
			var scene = new Scene("s2");
			scene.Push(new Entity("o2") { DrawScene = world["s1"] });
			scene.Push(new Entity("o3") { DrawScene = world["s1"] });
			world.Push(scene);
			var check = State.Serialization.SaveState(world);
			var world2 = State.Serialization.LoadState<World>(check);
			world2.Initialize(true);
			var secondScene = world2.Scenes.FirstOrDefault(s => s.ID == "s2");

			Assert.AreSame(secondScene, secondScene.GameObjectCache.Entities[0].UpdateScene);
			Assert.AreSame(secondScene, secondScene.GameObjectCache.Entities[1].UpdateScene);
		}

		[TestMethod]
		public void WorldComponentCacheAfterDeserializing()
		{
			var world = WorldTest.GetTestWorld();
			var state = STACK.State.Serialization.SaveState(world);
			var temp = STACK.State.Serialization.LoadState<World>(state);

			Assert.AreSame(world.Get<Mouse>(), world.Components.Where(i => i is Mouse).FirstOrDefault());
		}

		[TestMethod]
		public void WorldComponentsAfterDeserializing()
		{
			var world = WorldTest.GetTestWorld();
			world.Get<Camera>().Zoom = 2f;

			var bytes = State.Serialization.SaveState(world);
			var deserializedWorld = (World)State.Serialization.LoadState<World>(bytes);

			Assert.AreEqual(2f, deserializedWorld.Get<Camera>().Zoom);
		}
	}
}
