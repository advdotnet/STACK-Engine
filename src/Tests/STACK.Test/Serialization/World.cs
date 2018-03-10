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
            Entity Test = new Entity("stackobj");
            Test.Add<Transform>().Position = Vector2.UnitY;

            byte[] Check = STACK.State.Serialization.SaveState<Entity>(Test);

            Entity Second = STACK.State.Serialization.LoadState<Entity>(Check);
            Assert.AreEqual(Test.ID, Second.ID);
            Assert.AreEqual(Test.Items.Count, Second.Items.Count);
            Assert.AreEqual(Test.Get<Transform>().Position, Second.Get<Transform>().Position);
        }

        [TestMethod]
        public void TestSize()
        {
            Entity Test = new Entity("obj");
            System.Diagnostics.Debug.WriteLine(STACK.State.Serialization.SaveState<Entity>(Test).Length);
            Scene Stack = new Scene("stack");
            System.Diagnostics.Debug.WriteLine(STACK.State.Serialization.SaveState<Scene>(Stack).Length);
            Stack.Push(Test);
            System.Diagnostics.Debug.WriteLine(STACK.State.Serialization.SaveState<Scene>(Stack).Length);
        }

        [Serializable]
        class Derived : Entity
        {
            public List<string> List = new List<string>();
            public Derived(string id) : base(id) { }
        }

        [TestMethod]
        public void DerivedGameObjectState()
        {
            Derived Test = new Derived("derived");

            Test.List.Add("list-item");
            byte[] Check = STACK.State.Serialization.SaveState<Entity>(Test);
            Derived Second = (Derived)STACK.State.Serialization.LoadState<Entity>(Check);
            Assert.AreEqual(Test.ID, Second.ID);
            Assert.AreEqual(1, Second.List.Count);
            Assert.AreEqual(Test.List[0], Second.List[0]);
        }

        [TestMethod]
        public void ReferencesAfterDeserializing()
        {
            World World = WorldTest.GetTestWorld();
            Scene Scene = new Scene("s2");
            Scene.Push(new Entity("o2") { DrawScene = World["s1"] });
            Scene.Push(new Entity("o3") { DrawScene = World["s1"] });
            World.Push(Scene);
            byte[] Check = STACK.State.Serialization.SaveState<World>(World);
            World World2 = (World)STACK.State.Serialization.LoadState<World>(Check);
            World2.Initialize(true);
            Assert.AreSame(World2.Scenes[1], World2.Scenes[1].GameObjectCache.Entities[0].UpdateScene);
            Assert.AreSame(World2.Scenes[1], World2.Scenes[1].GameObjectCache.Entities[1].UpdateScene);
        }

        [TestMethod]
        public void WorldComponentCacheAfterDeserializing()
        {
            var World = WorldTest.GetTestWorld();
            var State = STACK.State.Serialization.SaveState<World>(World);
            var Temp = (World)STACK.State.Serialization.LoadState<World>(State);

            Assert.AreSame(World.Get<Mouse>(), World.Components.Where(i => i is Mouse).FirstOrDefault());
        }

        [TestMethod]
        public void WorldComponentsAfterDeserializing()
        {
            var World = WorldTest.GetTestWorld();
            World.Get<Camera>().Zoom = 2f;

            var Bytes = STACK.State.Serialization.SaveState<World>(World);
            var DeserializedWorld = (World)STACK.State.Serialization.LoadState<World>(Bytes);

            Assert.AreEqual(2f, DeserializedWorld.Get<Camera>().Zoom);
        }
    }
}
