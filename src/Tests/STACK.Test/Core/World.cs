using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using STACK.Components;
using STACK.Input;
using STACK.TestBase;
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
			var world = new World(ServiceProvider);
			var stack1 = new Scene("id1");
			var stack2 = new Scene("id1");
			world.Push(stack1);
			world.Push(stack2);
		}

		[TestMethod]
		public void GetsStackObject()
		{
			var world = new World(ServiceProvider);
			var stack = new Scene("id1");
			var testObject = new Entity("obj12");
			stack.Push(testObject);
			world.Push(stack);
			Assert.AreEqual(testObject, world.GetGameObject(testObject.ID));
		}

		[TestMethod]
		public void GetsObjectWithIDCache()
		{
			const int sceneCount = 10;
			const int entitiesPerSceneCount = 10;
			var watch = new Stopwatch();

			var world = new World(ServiceProvider);

			watch.Start();

			for (var i = 0; i < sceneCount; i++)
			{
				var scene = new Scene($"id{i}");

				for (var j = 0; j < entitiesPerSceneCount; j++)
				{
					var entity = new Entity($"id_s{i}_o{j}");
					scene.Push(entity);
				}

				world.Push(scene);
			}

			watch.Stop();
			Console.WriteLine($"Elapsed 1: {watch.ElapsedMilliseconds}");
			var rand = new Random();
			watch.Restart();

			for (var i = 0; i <= 1000000; i++)
			{
				world.GetGameObject(string.Format("id_s{0}_o{1}", rand.Next(sceneCount + 1), rand.Next(entitiesPerSceneCount + 1)));
			}

			Console.WriteLine($"Elapsed 2: {watch.ElapsedMilliseconds}");
		}

		[TestMethod]
		public void PopInvalidatesWorldCache()
		{
			const string entityId = "entity1";
			var world = new World(ServiceProvider);
			var scene = new Scene("stack1");

			world.Push(scene);
			world.Initialize(false);

			var e1 = new Entity(entityId);
			var e2 = new Entity(entityId);

			scene.Push(e1);
			Assert.AreEqual(e1, world.GetGameObject(entityId));

			scene.Pop(e1);
			Assert.AreEqual(null, world.GetGameObject(entityId));

			scene.Push(e2);
			Assert.AreEqual(e2, world.GetGameObject(entityId));
		}

		[TestMethod]
		public void PopInvalidatesSceneCacheInitialized()
		{
			const string entityId = "entity1";

			var scene = new Scene("stack1");
			scene.Initialize(false);

			var e1 = new Entity(entityId);
			var e2 = new Entity(entityId);

			scene.Push(e1);
			Assert.AreEqual(e1, scene.GetObject(entityId));

			scene.Pop(e1);
			Assert.AreEqual(null, scene.GetObject(entityId));

			scene.Push(e2);
			Assert.AreEqual(e2, scene.GetObject(entityId));
		}

		[TestMethod]
		public void PopInvalidatesSceneCacheUnInitialized()
		{
			const string entityId = "entity1";

			var scene = new Scene("stack1");

			var e1 = new Entity(entityId);
			var e2 = new Entity(entityId);

			scene.Push(e1);
			Assert.AreEqual(e1, scene.GetObject(entityId));

			scene.Pop(e1);
			Assert.AreEqual(null, scene.GetObject(entityId));

			scene.Push(e2);
			Assert.AreEqual(e2, scene.GetObject(entityId));
		}

		[TestMethod]
		public void GetsNeighbors()
		{
			var world = new World(ServiceProvider);
			var stack1 = new Scene("stack1");

			var p1 = new Entity("portal1"); p1.Add<Exit>().TargetEntrance = "door2";
			var p2 = new Entity("portal2"); p2.Add<Exit>().TargetEntrance = "door3";

			stack1.Push(p1, p2);
			var stack2 = new Scene("stack2");
			stack2.Push(new Entity("door2"));
			var stack3 = new Scene("stack3");
			stack3.Push(new Entity("door3"));

			world.Push(stack1, stack2, stack3);

			var neighbors = world.GetSceneNeighbors(stack1);
			Assert.AreEqual(2, neighbors.Count);
			Assert.IsTrue(neighbors.Contains(world["stack2"]));
			Assert.IsTrue(neighbors.Contains(world["stack3"]));
		}

		[TestMethod]
		public void FindsPath()
		{
			var world = new World(ServiceProvider);

			var stack1 = new Scene("s1"); stack1.Push(new Entity("t1"));
			var stack2 = new Scene("s2"); stack2.Push(new Entity("t2"));
			var stack3 = new Scene("s3"); stack3.Push(new Entity("t3"));
			var stack4 = new Scene("s4"); stack4.Push(new Entity("t4"));

			var p1 = new Entity("p1"); p1.Add<Exit>().TargetEntrance = "t2"; stack1.Push(p1);
			var p2 = new Entity("p2"); p2.Add<Exit>().TargetEntrance = "t3"; stack2.Push(p2);
			var p3 = new Entity("p3"); p3.Add<Exit>().TargetEntrance = "t4"; stack3.Push(p3);
			var p4 = new Entity("p4"); p4.Add<Exit>().TargetEntrance = "t2"; stack4.Push(p4);

			world.Push(stack1, stack2, stack3, stack4);

			var path1 = new List<string>();
			world.FindPath("s1", "s4", ref path1);
			var path2 = new List<string>();
			world.FindPath("s4", "s3", ref path2);
			var path3 = new List<string>();
			world.FindPath("s3", "s1", ref path3);

			Assert.AreEqual(4, path1.Count);

			CollectionAssert.AreEqual(new List<string>() { "s1", "s2", "s3", "s4" }, path1);
			Assert.AreEqual(3, path2.Count);

			CollectionAssert.AreEqual(new List<string>() { "s4", "s2", "s3" }, path2);
			Assert.AreEqual(0, path3.Count);
		}

		[TestMethod]
		public void InitializeUpdatesStackPriority()
		{
			var world = new World(ServiceProvider);
			world.Push(new Scene("s1") { DrawOrder = 1 }, new Scene("s2") { DrawOrder = 5 });
			Assert.AreEqual("s2", world.Scenes[0].ID);
			Assert.AreEqual("s1", world.Scenes[1].ID);
			world.Initialize(false);
			Assert.AreEqual("s2", world.Scenes[0].ID);
			Assert.AreEqual("s1", world.Scenes[1].ID);
		}

		[TestMethod]
		public void PriorityChangeBubblesUp()
		{
			var world = new World(ServiceProvider);
			world.Push(new Scene("s1") { DrawOrder = 2 }, new Scene("s2") { DrawOrder = 1 });
			Assert.AreEqual("s1", world.Scenes[0].ID);
			Assert.AreEqual("s2", world.Scenes[1].ID);
			world["s2"].DrawOrder = 3;
			Assert.AreEqual("s2", world.Scenes[0].ID);
			Assert.AreEqual("s1", world.Scenes[1].ID);
		}

		public static World GetTestWorld(InputProvider input = null)
		{
			var world = new World(ServiceProvider, input);
			var scene = new Scene("s1") { Enabled = true };

			var o1 = new Entity("o1");

			HotspotRectangle
				.Create(o1)
				.SetRectangle(5, 5, 5, 5);

			scene.Push(o1);

			world.Push(scene);
			world.Initialize(false);

			return world;
		}

		[TestMethod]
		public void GetsObjectUnderMouse()
		{
			var input = new TestInputProvider();
			var world = GetTestWorld(input);

			input.MouseMove(6, 6);
			input.Dispatch();

			Assert.AreEqual(world.GetGameObject("o1"), world.Get<Mouse>().ObjectUnderMouse);
		}

		[TestMethod]
		public void NoneUnderMouse()
		{
			var input = new TestInputProvider();
			var world = GetTestWorld(input);

			input.MouseMove(4, 4);
			input.Dispatch();

			Assert.AreEqual(null, world.Get<Mouse>().ObjectUnderMouse);
		}

		[TestMethod]
		public void GetsHigherPriorityObjectUnderMouse()
		{
			var input = new TestInputProvider();
			var world = GetTestWorld(input);

			// push another layer with higher priority
			var stack2 = new Scene("s2") { Enabled = true, DrawOrder = 5 };

			var o2 = new Entity("o2");
			o2.Add<HotspotRectangle>().SetRectangle(5, 5, 5, 5);

			stack2.Push(o2);

			world.Push(stack2);
			world.UpdatePriority();

			input.MouseMove(6, 6);
			input.Dispatch();

			Assert.AreEqual(world.GetGameObject("o2"), world.Get<Mouse>().ObjectUnderMouse);
		}

		[TestMethod]
		public void InteractiveMousePosition()
		{
			var input = new TestInputProvider();
			var world = GetTestWorld(input);
			input.MouseMove(5, 5);
			input.Dispatch();
			Assert.AreEqual(new Vector2(5, 5), world.Get<Mouse>().Position);
			world.Interactive = false;
			input.MouseMove(2, 7);
			input.Dispatch();
			world.Interactive = true;
			Assert.AreEqual(new Vector2(2, 7), world.Get<Mouse>().Position);
		}

		[TestMethod]
		public void InteractiveMouseClick()
		{
			var input = new TestInputProvider();
			var world = GetTestWorld(input);
			world.Push(new ClickScene());
			input.MouseMove(52, 52);
			input.Dispatch();

			world.Interactive = false;
			input.MouseClick();
			input.Dispatch();
		}

		private class ClickScene : Scene
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
