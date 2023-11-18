using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using STACK.Components;
using STACK.TestBase;
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
			var scene = new Scene();
			var entity = new Entity();
			scene.Initialize(false);
			scene.Push(entity);
			Assert.AreEqual(1, scene.GameObjectCache.Entities.Count);
			scene.Pop(entity);
			Assert.AreEqual(0, scene.GameObjectCache.Entities.Count);
		}

		[TestMethod]
		public void VisibleObjectCacheTest()
		{
			var scene = new Scene();
			var visibleEntity = new Entity() { Visible = true };
			var invisibleEntity = new Entity() { Visible = false };
			scene.Push(visibleEntity);
			scene.Push(invisibleEntity);
			Assert.AreEqual(1, scene.GameObjectCache.VisibleObjects.Count);
		}

		[TestMethod]
		public void VisibleObjectsCacheOrderTest()
		{
			var scene = new Scene();
			scene.Initialize(false);
			var firstEntity = new Entity() { DrawOrder = 1 };
			var secondEntity = new Entity() { DrawOrder = 2 };
			scene.Push(firstEntity, secondEntity);
			Assert.AreEqual(scene.GameObjectCache.VisibleObjects.First(), secondEntity);
			Assert.AreEqual(scene.GameObjectCache.VisibleObjects[1], firstEntity);

			Assert.AreEqual(scene.GameObjectCache.ObjectsToDraw[0], firstEntity);
			Assert.AreEqual(scene.GameObjectCache.ObjectsToDraw[1], secondEntity);

			firstEntity.DrawOrder = 3;
			Assert.AreEqual(scene.GameObjectCache.VisibleObjects.First(), firstEntity);
			Assert.AreEqual(scene.GameObjectCache.VisibleObjects[1], secondEntity);

			Assert.AreEqual(scene.GameObjectCache.ObjectsToDraw[0], secondEntity);
			Assert.AreEqual(scene.GameObjectCache.ObjectsToDraw[1], firstEntity);
		}

		[TestMethod]
		public void VisibleObjectsCacheOrderDifferentScenesTest()
		{
			var updateScene = new Scene("1");
			var drawScene = new Scene("2");
			var world = new World(new TestServiceProvider());
			world.Push(updateScene, drawScene);
			world.Initialize(false);

			var firstEntity = new Entity() { DrawOrder = 1 };
			var secondEntity = new Entity() { DrawOrder = 2 };
			updateScene.Push(firstEntity, secondEntity);

			firstEntity.DrawScene = drawScene;
			secondEntity.DrawScene = drawScene;

			Assert.AreEqual(updateScene.GameObjectCache.VisibleObjects.Count, 0);
			Assert.AreEqual(updateScene.GameObjectCache.ObjectsToDraw.Count, 0);

			Assert.AreEqual(drawScene.GameObjectCache.VisibleObjects.First(), secondEntity);
			Assert.AreEqual(drawScene.GameObjectCache.VisibleObjects[1], firstEntity);

			Assert.AreEqual(drawScene.GameObjectCache.ObjectsToDraw[0], firstEntity);
			Assert.AreEqual(drawScene.GameObjectCache.ObjectsToDraw[1], secondEntity);

			firstEntity.DrawOrder = 3;

			Assert.AreEqual(updateScene.GameObjectCache.VisibleObjects.Count, 0);
			Assert.AreEqual(updateScene.GameObjectCache.ObjectsToDraw.Count, 0);

			Assert.AreEqual(drawScene.GameObjectCache.VisibleObjects.First(), firstEntity);
			Assert.AreEqual(drawScene.GameObjectCache.VisibleObjects[1], secondEntity);

			Assert.AreEqual(drawScene.GameObjectCache.ObjectsToDraw[0], secondEntity);
			Assert.AreEqual(drawScene.GameObjectCache.ObjectsToDraw[1], firstEntity);
		}

		[TestMethod]
		public void SetsNameSpaceAsID()
		{
			var scene = new Room1.MyScene();
			Assert.AreEqual("STACK.Test.Room1.MyScene", scene.ID);
		}

		[TestMethod]
		public void KeepsManualID()
		{
			var scene = new Scene("myID");
			Assert.AreEqual("myID", scene.ID);
		}

		[TestMethod]
		public void GetsPortalsTo()
		{
			var world = new World(new TestServiceProvider());

			var stack1 = new Scene("s1");
			var stack2 = new Scene("s2"); stack2.Push(new Entity("t2"));
			var stack3 = new Scene("s3"); stack3.Push(new Entity("t3"));

			var p1 = new Entity("p1"); p1.Add<Exit>().TargetEntrance = "t2";
			var p2 = new Entity("p2"); p2.Add<Exit>().TargetEntrance = "t2";
			var p3 = new Entity("p3"); p3.Add<Exit>().TargetEntrance = "t3";

			stack1.Push(p1);
			stack1.Push(p2);
			stack1.Push(p3);

			world.Push(stack1, stack2, stack3);

			var results = new List<Exit>();
			stack1.GetPassagesTo(stack2, ref results);
			CollectionAssert.AreEqual(new List<Exit>() { stack1["p1"].Get<Exit>(), stack1["p2"].Get<Exit>() }, results);
			stack1.GetPassagesTo("s2", ref results);
			CollectionAssert.AreEqual(new List<Exit>() { stack1["p1"].Get<Exit>(), stack1["p2"].Get<Exit>() }, results);
			stack1.GetPassagesTo("s3", ref results);
			CollectionAssert.AreEqual(new List<Exit>() { stack1["p3"].Get<Exit>() }, results);
			stack1.GetPassagesTo("s4", ref results);
			CollectionAssert.AreEqual(new List<Exit>(), results);
		}

		[TestMethod]
		public void GetsObject()
		{
			var stack1 = new Scene("s1");
			var object1 = new Entity("o1");
			stack1.Push(object1);
			Assert.AreEqual(null, stack1["o2"]);
			Assert.AreEqual(null, stack1.GetObject("o2"));
			Assert.AreEqual(object1, stack1["o1"]);
			Assert.AreEqual(object1, stack1.GetObject("o1"));
		}

		[TestMethod]
		public void GetsHitObject()
		{
			var stack1 = new Scene("s1");
			var object1 = new Entity("o1");

			HotspotRectangle
				.Create(object1)
				.SetRectangle(0, 0, 10, 10);

			stack1.Push(object1);

			var hitObject = stack1.GetHitObject(new Vector2(5, 5));
			Assert.AreEqual(object1, hitObject);

			var test = State.Serialization.SaveState(stack1);
			Trace.WriteLine(test.Length); // 2917
			Trace.WriteLine(System.Text.Encoding.Default.GetString(test));
		}

		[TestMethod]
		public void PushSetsScene()
		{
			var entity = new Entity();
			var scene = new Scene("s1");
			scene.Push(entity);
			Assert.AreEqual(scene, entity.DrawScene);
			Assert.AreEqual(scene, entity.UpdateScene);
		}

		[TestMethod]
		public void UpdateScene()
		{
			var entity = new Entity();
			var scene = new Scene("s1");
			scene.Push(entity);
			var scene2 = new Scene("s2");
			entity.UpdateScene = scene2;
			Assert.AreEqual(scene, entity.DrawScene);
			Assert.AreEqual(scene2, entity.UpdateScene);
		}

		private class NotifiedEntity : Entity
		{
			public bool Notified { get; private set; }
			public override void OnNotify<T>(string message, T data)
			{
				if (message == "notification")
				{
					Notified = true;
				}
				base.OnNotify(message, data);
			}
		}

		[TestMethod]
		public void SceneNotifiesDrawnObjects()
		{
			var world = new World(new TestServiceProvider());
			var entity1 = new Entity("e1");
			var entity2 = new NotifiedEntity();
			var scene1 = new Scene("s1");
			var scene2 = new Scene("s2");
			world.Push(scene1, scene2);
			scene1.Push(entity1);
			scene2.Push(entity2);
			world.Initialize(false);
			entity2.EnterScene(scene1);
			scene1.Notify<object>("notification", null);
			Assert.IsTrue(entity2.Notified);
		}

	}
}
