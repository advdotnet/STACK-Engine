using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using STACK.Components;
using STACK.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace STACK.Test
{

    [TestClass]
    public class WorldTest
    {
        public static IServiceProvider ServiceProvider = new TestServiceProvider();

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PushSameIDThrows()
        {
            var World = new World(ServiceProvider);
            var Stack1 = new Scene("id1");
            var Stack2 = new Scene("id1");
            World.Push(Stack1);
            World.Push(Stack2);
        }

        [TestMethod]
        public void GetsStackObject()
        {
            var World = new World(ServiceProvider);
            var Stack = new Scene("id1");
            var TestObject = new Entity("obj12");
            Stack.Push(TestObject);
            World.Push(Stack);
            Assert.AreEqual(TestObject, World.GetGameObject(TestObject.ID));
        }

        [TestMethod]
        public void GetsObjectWithIDCache()
        {
            const int SCENECOUNT = 10;
            const int ENTITESPERSCENECOUNT = 10;
            var Watch = new Stopwatch();

            var World = new World(ServiceProvider);

            Watch.Start();

            for (int i = 0; i < SCENECOUNT; i++)
            {
                var Scene = new Scene("id" + i);

                for (int j = 0; j < ENTITESPERSCENECOUNT; j++)
                {
                    var Entity = new Entity("id_s" + i + "_o" + j);
                    Scene.Push(Entity);
                }

                World.Push(Scene);
            }

            Watch.Stop();
            Console.WriteLine("Elapsed 1: " + Watch.ElapsedMilliseconds);
            var Rand = new Random();
            Watch.Restart();

            for (int i = 0; i <= 1000000; i++)
            {
                World.GetGameObject(string.Format("id_s{0}_o{1}", Rand.Next(SCENECOUNT + 1), Rand.Next(ENTITESPERSCENECOUNT + 1)));
            }

            Console.WriteLine("Elapsed 2: " + Watch.ElapsedMilliseconds);
        }


        [TestMethod]
        public void GetsNeighbors()
        {
            var World = new World(ServiceProvider);
            var Stack1 = new Scene("stack1");

            var p1 = new Entity("portal1"); p1.Add<Exit>().TargetEntrance = "door2";
            var p2 = new Entity("portal2"); p2.Add<Exit>().TargetEntrance = "door3";

            Stack1.Push(p1, p2);
            Scene Stack2 = new Scene("stack2");
            Stack2.Push(new Entity("door2"));
            Scene Stack3 = new Scene("stack3");
            Stack3.Push(new Entity("door3"));

            World.Push(Stack1, Stack2, Stack3);

            var Neighbors = World.GetSceneNeighbors(Stack1);
            Assert.AreEqual(2, Neighbors.Count);
            Assert.IsTrue(Neighbors.Contains(World["stack2"]));
            Assert.IsTrue(Neighbors.Contains(World["stack3"]));
        }

        [TestMethod]
        public void FindsPath()
        {
            var World = new World(ServiceProvider);

            var Stack1 = new Scene("s1"); Stack1.Push(new Entity("t1"));
            var Stack2 = new Scene("s2"); Stack2.Push(new Entity("t2"));
            var Stack3 = new Scene("s3"); Stack3.Push(new Entity("t3"));
            var Stack4 = new Scene("s4"); Stack4.Push(new Entity("t4"));

            var p1 = new Entity("p1"); p1.Add<Exit>().TargetEntrance = "t2"; Stack1.Push(p1);
            var p2 = new Entity("p2"); p2.Add<Exit>().TargetEntrance = "t3"; Stack2.Push(p2);
            var p3 = new Entity("p3"); p3.Add<Exit>().TargetEntrance = "t4"; Stack3.Push(p3);
            var p4 = new Entity("p4"); p4.Add<Exit>().TargetEntrance = "t2"; Stack4.Push(p4);

            World.Push(Stack1, Stack2, Stack3, Stack4);

            var Path1 = new List<string>();
            World.FindPath("s1", "s4", ref Path1);
            var Path2 = new List<string>();
            World.FindPath("s4", "s3", ref Path2);
            var Path3 = new List<string>();
            World.FindPath("s3", "s1", ref Path3);

            Assert.AreEqual(4, Path1.Count);

            CollectionAssert.AreEqual(new List<string>() { "s1", "s2", "s3", "s4" }, Path1);
            Assert.AreEqual(3, Path2.Count);

            CollectionAssert.AreEqual(new List<string>() { "s4", "s2", "s3" }, Path2);
            Assert.AreEqual(0, Path3.Count);
        }

        [TestMethod]
        public void InitializeUpdatesStackPriority()
        {
            var World = new World(ServiceProvider);
            World.Push(new Scene("s1") { DrawOrder = 1 }, new Scene("s2") { DrawOrder = 5 });
            Assert.AreEqual("s2", World.Scenes[0].ID);
            Assert.AreEqual("s1", World.Scenes[1].ID);
            World.Initialize(false);
            Assert.AreEqual("s2", World.Scenes[0].ID);
            Assert.AreEqual("s1", World.Scenes[1].ID);
        }

        [TestMethod]
        public void PriorityChangeBubblesUp()
        {
            var World = new World(ServiceProvider);
            World.Push(new Scene("s1") { DrawOrder = 2 }, new Scene("s2") { DrawOrder = 1 });
            Assert.AreEqual("s1", World.Scenes[0].ID);
            Assert.AreEqual("s2", World.Scenes[1].ID);
            World["s2"].DrawOrder = 3;
            Assert.AreEqual("s2", World.Scenes[0].ID);
            Assert.AreEqual("s1", World.Scenes[1].ID);
        }

        public static World GetTestWorld(InputProvider input = null)
        {
            var World = new World(ServiceProvider, input);
            var Scene = new Scene("s1") { Enabled = true };

            var o1 = new Entity("o1");

            HotspotRectangle
                .Create(o1)
                .SetRectangle(5, 5, 5, 5);

            Scene.Push(o1);

            World.Push(Scene);
            World.Initialize(false);

            return World;
        }

        [TestMethod]
        public void GetsObjectUnderMouse()
        {
            var Input = new TestInputProvider();
            var World = GetTestWorld(Input);

            Input.MouseMove(6, 6);
            Input.Dispatch();

            Assert.AreEqual(World.GetGameObject("o1"), World.Get<Mouse>().ObjectUnderMouse);
        }

        [TestMethod]
        public void NoneUnderMouse()
        {
            var Input = new TestInputProvider();
            var World = GetTestWorld(Input);

            Input.MouseMove(4, 4);
            Input.Dispatch();

            Assert.AreEqual(null, World.Get<Mouse>().ObjectUnderMouse);
        }

        [TestMethod]
        public void GetsHigherPriorityObjectUnderMouse()
        {
            var Input = new TestInputProvider();
            var World = GetTestWorld(Input);

            // push another layer with higher priority
            Scene Stack2 = new Scene("s2") { Enabled = true, DrawOrder = 5 };

            var o2 = new Entity("o2");
            o2.Add<HotspotRectangle>().SetRectangle(5, 5, 5, 5);

            Stack2.Push(o2);

            World.Push(Stack2);
            World.UpdatePriority();

            Input.MouseMove(6, 6);
            Input.Dispatch();

            Assert.AreEqual(World.GetGameObject("o2"), World.Get<Mouse>().ObjectUnderMouse);
        }

        [TestMethod]
        public void InteractiveMousePosition()
        {
            var Input = new TestInputProvider();
            var World = GetTestWorld(Input);
            Input.MouseMove(5, 5);
            Input.Dispatch();
            Assert.AreEqual(new Vector2(5, 5), World.Get<Mouse>().Position);
            World.Interactive = false;
            Input.MouseMove(2, 7);
            Input.Dispatch();
            World.Interactive = true;
            Assert.AreEqual(new Vector2(2, 7), World.Get<Mouse>().Position);
        }

        [TestMethod]
        public void InteractiveMouseClick()
        {
            var Input = new TestInputProvider();
            var World = GetTestWorld(Input);
            World.Push(new ClickScene());
            Input.MouseMove(52, 52);
            Input.Dispatch();

            World.Interactive = false;
            Input.MouseClick();
            Input.Dispatch();
        }

        class ClickScene : Scene
        {
            public ClickScene()
            {
                Enabled = true;

                InputDispatcher
                    .Create(this)
                    .SetOnMouseUpFn(OnMouseUp);
            }

            public void OnMouseUp(Vector2 mousePosition, MouseButton button)
            {
                Assert.Fail("Click event propagated");
            }
        }

    }
}
