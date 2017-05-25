using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using STACK.Components;
using StarFinder;

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

            byte[] Check = STACK.State.State.SaveState<Entity>(Test);

            Entity Second = STACK.State.State.LoadState<Entity>(Check);
            Assert.AreEqual(Test.ID, Second.ID);
            Assert.AreEqual(Test.Items.Count, Second.Items.Count);
            Assert.AreEqual(Test.Get<Transform>().Position, Second.Get<Transform>().Position);
        }

        [TestMethod]
        public void TestSize()
        {
            Entity Test = new Entity("obj");
            System.Diagnostics.Debug.WriteLine(STACK.State.State.SaveState<Entity>(Test).Length);
            Scene Stack = new Scene("stack");
            System.Diagnostics.Debug.WriteLine(STACK.State.State.SaveState<Scene>(Stack).Length);
            Stack.Push(Test);
            System.Diagnostics.Debug.WriteLine(STACK.State.State.SaveState<Scene>(Stack).Length);
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
            byte[] Check = STACK.State.State.SaveState<Entity>(Test);
            Derived Second = (Derived)STACK.State.State.LoadState<Entity>(Check);
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
            byte[] Check = STACK.State.State.SaveState<World>(World);
            World World2 = (World)STACK.State.State.LoadState<World>(Check);
            Assert.AreSame(World2.Scenes[1], World2.Scenes[1].Entities[0].UpdateScene);
            Assert.AreSame(World2.Scenes[1], World2.Scenes[1].Entities[1].UpdateScene);
        }

        [TestMethod]
        public void WorldComponentCacheAfterDeserializing()
        {
            World World = WorldTest.GetTestWorld();            
            var State = STACK.State.State.SaveState<World>(World);
            World Temp = (World)STACK.State.State.LoadState<World>(State);

            World.RestoreState(Temp, null, null);

            Assert.AreSame(World.Get<Mouse>(), World.Items.Where(i => i is Mouse).FirstOrDefault());
        } 
    }
}
