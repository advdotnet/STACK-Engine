using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using STACK.Components;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace STACK.Test.Room1
{
    public class MyScene : Scene { }
}

namespace STACK.Test
{
    [TestClass]
    public class SceneTest
    {

        [TestMethod]
        public void EntityCacheTest()
        {
            var Scene = new Scene();
            var Entity = new Entity();
            Scene.Initialize(false);
            Scene.Push(Entity);
            Assert.AreEqual(1, Scene.GameObjectCache.Entities.Count);
            Scene.Pop(Entity);
            Assert.AreEqual(0, Scene.GameObjectCache.Entities.Count);
        }

        [TestMethod]
        public void VisibleObjectCacheTest()
        {
            var Scene = new Scene();
            var VisibleEntity = new Entity() { Visible = true };
            var InvisibleEntity = new Entity() { Visible = false };
            Scene.Push(VisibleEntity);
            Scene.Push(InvisibleEntity);
            Assert.AreEqual(1, Scene.GameObjectCache.VisibleObjects.Count);
        }

        [TestMethod]
        public void VisibleObjectsCacheOrderTest()
        {
            var Scene = new Scene();
            Scene.Initialize(false);
            var FirstEntity = new Entity() { DrawOrder = 1 };
            var SecondEntity = new Entity() { DrawOrder = 2 };
            Scene.Push(FirstEntity, SecondEntity);
            Assert.AreEqual(Scene.GameObjectCache.VisibleObjects.First(), SecondEntity);
            Assert.AreEqual(Scene.GameObjectCache.VisibleObjects[1], FirstEntity);

            Assert.AreEqual(Scene.GameObjectCache.ObjectsToDraw[0], FirstEntity);
            Assert.AreEqual(Scene.GameObjectCache.ObjectsToDraw[1], SecondEntity);

            FirstEntity.DrawOrder = 3;
            Assert.AreEqual(Scene.GameObjectCache.VisibleObjects.First(), FirstEntity);
            Assert.AreEqual(Scene.GameObjectCache.VisibleObjects[1], SecondEntity);

            Assert.AreEqual(Scene.GameObjectCache.ObjectsToDraw[0], SecondEntity);
            Assert.AreEqual(Scene.GameObjectCache.ObjectsToDraw[1], FirstEntity);
        }

        [TestMethod]
        public void VisibleObjectsCacheOrderDifferentScenesTest()
        {
            var UpdateScene = new Scene("1");
            var DrawScene = new Scene("2");
            var World = new World(new TestServiceProvider());
            World.Push(UpdateScene, DrawScene);
            World.Initialize(false);

            var FirstEntity = new Entity() { DrawOrder = 1 };
            var SecondEntity = new Entity() { DrawOrder = 2 };
            UpdateScene.Push(FirstEntity, SecondEntity);

            FirstEntity.DrawScene = DrawScene;
            SecondEntity.DrawScene = DrawScene;

            Assert.AreEqual(UpdateScene.GameObjectCache.VisibleObjects.Count, 0);
            Assert.AreEqual(UpdateScene.GameObjectCache.ObjectsToDraw.Count, 0);

            Assert.AreEqual(DrawScene.GameObjectCache.VisibleObjects.First(), SecondEntity);
            Assert.AreEqual(DrawScene.GameObjectCache.VisibleObjects[1], FirstEntity);

            Assert.AreEqual(DrawScene.GameObjectCache.ObjectsToDraw[0], FirstEntity);
            Assert.AreEqual(DrawScene.GameObjectCache.ObjectsToDraw[1], SecondEntity);

            FirstEntity.DrawOrder = 3;

            Assert.AreEqual(UpdateScene.GameObjectCache.VisibleObjects.Count, 0);
            Assert.AreEqual(UpdateScene.GameObjectCache.ObjectsToDraw.Count, 0);

            Assert.AreEqual(DrawScene.GameObjectCache.VisibleObjects.First(), FirstEntity);
            Assert.AreEqual(DrawScene.GameObjectCache.VisibleObjects[1], SecondEntity);

            Assert.AreEqual(DrawScene.GameObjectCache.ObjectsToDraw[0], SecondEntity);
            Assert.AreEqual(DrawScene.GameObjectCache.ObjectsToDraw[1], FirstEntity);
        }

        [TestMethod]
        public void SetsNameSpaceAsID()
        {
            Room1.MyScene Scene = new Room1.MyScene();
            Assert.AreEqual("STACK.Test.Room1.MyScene", Scene.ID);
        }

        [TestMethod]
        public void KeepsManualID()
        {
            Scene Scene = new STACK.Scene("myID");
            Assert.AreEqual("myID", Scene.ID);
        }

        [TestMethod]
        public void GetsPortalsTo()
        {
            World World = new World(new TestServiceProvider());

            Scene Stack1 = new Scene("s1");
            Scene Stack2 = new Scene("s2"); Stack2.Push(new Entity("t2"));
            Scene Stack3 = new Scene("s3"); Stack3.Push(new Entity("t3"));

            var p1 = new Entity("p1"); p1.Add<Exit>().TargetEntrance = "t2";
            var p2 = new Entity("p2"); p2.Add<Exit>().TargetEntrance = "t2";
            var p3 = new Entity("p3"); p3.Add<Exit>().TargetEntrance = "t3";

            Stack1.Push(p1);
            Stack1.Push(p2);
            Stack1.Push(p3);

            World.Push(Stack1, Stack2, Stack3);

            var Results = new List<Exit>();
            Stack1.GetPassagesTo(Stack2, ref Results);
            CollectionAssert.AreEqual(new List<Exit>() { Stack1["p1"].Get<Exit>(), Stack1["p2"].Get<Exit>() }, Results);
            Stack1.GetPassagesTo("s2", ref Results);
            CollectionAssert.AreEqual(new List<Exit>() { Stack1["p1"].Get<Exit>(), Stack1["p2"].Get<Exit>() }, Results);
            Stack1.GetPassagesTo("s3", ref Results);
            CollectionAssert.AreEqual(new List<Exit>() { Stack1["p3"].Get<Exit>() }, Results);
            Stack1.GetPassagesTo("s4", ref Results);
            CollectionAssert.AreEqual(new List<Exit>(), Results);
        }


        [TestMethod]
        public void GetsObject()
        {
            Scene Stack1 = new Scene("s1");
            Entity Object1 = new Entity("o1");
            Stack1.Push(Object1);
            Assert.AreEqual(null, Stack1["o2"]);
            Assert.AreEqual(null, Stack1.GetObject("o2"));
            Assert.AreEqual(Object1, Stack1["o1"]);
            Assert.AreEqual(Object1, Stack1.GetObject("o1"));
        }

        [TestMethod]
        public void GetsHitObject()
        {
            Scene Stack1 = new Scene("s1");
            Entity Object1 = new Entity("o1");

            HotspotRectangle
                .Create(Object1)
                .SetRectangle(0, 0, 10, 10);

            Stack1.Push(Object1);

            var HitObject = Stack1.GetHitObject(new Vector2(5, 5));
            Assert.AreEqual(Object1, HitObject);

            var Test = STACK.State.Serialization.SaveState<Scene>(Stack1);
            Trace.WriteLine(Test.Length); // 2917
            Trace.WriteLine(System.Text.Encoding.Default.GetString(Test));
        }

        [TestMethod]
        public void PushSetsScene()
        {
            Entity Entity = new Entity();
            Scene Scene = new Scene("s1");
            Scene.Push(Entity);
            Assert.AreEqual(Scene, Entity.DrawScene);
            Assert.AreEqual(Scene, Entity.UpdateScene);
        }

        [TestMethod]
        public void UpdateScene()
        {
            Entity Entity = new Entity();
            Scene Scene = new Scene("s1");
            Scene.Push(Entity);
            Scene Scene2 = new Scene("s2");
            Entity.UpdateScene = Scene2;
            Assert.AreEqual(Scene, Entity.DrawScene);
            Assert.AreEqual(Scene2, Entity.UpdateScene);
        }

        class NotifiedEntity : Entity
        {
            public bool Notified { get; private set; }
            public override void OnNotify<T>(string message, T data)
            {
                if (message == "notification")
                {
                    Notified = true;
                }
                base.OnNotify<T>(message, data);
            }
        }

        [TestMethod]
        public void SceneNotifiesDrawnObjects()
        {
            var World = new World(new TestServiceProvider());
            var Entity1 = new Entity("e1");
            var Entity2 = new NotifiedEntity();
            var Scene1 = new Scene("s1");
            var Scene2 = new Scene("s2");
            World.Push(Scene1, Scene2);
            Scene1.Push(Entity1);
            Scene2.Push(Entity2);
            World.Initialize(false);
            Entity2.EnterScene(Scene1);
            Scene1.Notify<object>("notification", null);
            Assert.IsTrue(Entity2.Notified);
        }

    }
}
