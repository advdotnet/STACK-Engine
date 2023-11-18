using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using STACK.Components;
using System;
using System.Collections;

namespace STACK.Test
{

	[TestClass]
	public class ScriptsTest
	{
		[TestMethod]
		public void ScriptEngineAddScript()
		{
			var testScriptEngine = new Scripts();
			testScriptEngine.Start(new EnumerableClass().Foo());
			Assert.AreEqual(1, testScriptEngine.ScriptCollection.Count);
		}

		[TestMethod]
		public void ScriptEngineClearScripts()
		{
			var testScriptEngine = new Scripts();
			testScriptEngine.Start(new EnumerableClass().Foo());
			testScriptEngine.Update();
			testScriptEngine.Clear();
			Assert.AreEqual(0, testScriptEngine.ScriptCollection.Count);
		}

		[TestMethod]
		public void ExecuteImmediately()
		{
			var testScriptEngine = new Scripts();
			var enumerable = new EnumerableClass();
			testScriptEngine.Start(enumerable.YieldOnce(), "");
			Assert.IsTrue(enumerable.Yielded);
			testScriptEngine.Update();
			Assert.AreEqual(0, testScriptEngine.ScriptCollection.Count);
		}

		[TestMethod]
		public void ExecuteDeferred()
		{
			var testScriptEngine = new Scripts();
			var enumerable = new EnumerableClass();
			testScriptEngine.Enqueue(enumerable.YieldOnce(), "");
			Assert.IsFalse(enumerable.Yielded);
			testScriptEngine.Update();
			Assert.IsTrue(enumerable.Yielded);
			testScriptEngine.Update();
			Assert.AreEqual(0, testScriptEngine.ScriptCollection.Count);
		}

		[TestMethod]
		public void ScriptEngineNestedScript()
		{
			var testScriptEngine = new Scripts();
			var enumerableClass = new EnumerableClass();
			testScriptEngine.Start(enumerableClass.NestedScript(testScriptEngine), "");
			Assert.AreEqual(1, testScriptEngine.ScriptCollection.Count);
			testScriptEngine.Update();
			Assert.AreEqual(2, testScriptEngine.ScriptCollection.Count);
			testScriptEngine.Update();
			Assert.IsFalse(enumerableClass.FinishedNestedScript);
			testScriptEngine.Update();
			Assert.AreEqual(0, testScriptEngine.ScriptCollection.Count);
			Assert.IsTrue(enumerableClass.FinishedNestedScript);
		}

		[TestMethod]
		public void ClearSetsScriptDone()
		{
			var testScriptEngine = new Scripts();
			var enumerable = new EnumerableClass();
			var script = testScriptEngine.Start(enumerable.Foo(), "ScriptName");
			Assert.IsFalse(script.Done);
			testScriptEngine.Update();
			Assert.IsFalse(script.Done);
			testScriptEngine.Clear();
			Assert.IsTrue(script.Done);
		}

		[TestMethod]
		public void RemoveSetsScriptDone()
		{
			var testScriptEngine = new Scripts();
			var enumerable = new EnumerableClass();
			var script = testScriptEngine.Start(enumerable.Foo(), "ScriptName");
			Assert.IsFalse(script.Done);
			testScriptEngine.Update();
			Assert.IsFalse(script.Done);
			testScriptEngine.Remove("ScriptName");
			Assert.IsTrue(script.Done);
		}

		[TestMethod]
		public void NestedIEnumerator()
		{
			var script = new Scripts();
			var scriptClass = new EnumerableClass();
			script.Start(scriptClass.NestedIEnumerator());
			while (script.ScriptCollection.Count > 0)
			{
				script.Update();
			}
			Assert.AreEqual(7, scriptClass.TestInt);
		}

		/// <summary>
		/// Checks that for a high walking speed the goto script does not get stuck in a loop.
		/// </summary>
		[TestMethod]
		public void HighWalkSpeedLoop()
		{
			var entity = new Entity();

			Transform.Create(entity).SetSpeed(800);
			Scripts.Create(entity);
			Navigation.Create(entity).SetPath(CreateRectangularPath(100));

			entity.Get<Scripts>().GoTo(50, 50);

			Vector2 lastPosition;
			var i = 0;

			do
			{
				lastPosition = entity.Get<Transform>().Position;
				entity.Update();

				if (++i > 10)
				{
					throw new Exception("Walking for too many updates");
				}
			} while (lastPosition != entity.Get<Transform>().Position);
		}

		public static Path CreateRectangularPath(int size)
		{
			var points = new PathVertex[4];

			points[0] = new PathVertex(0, 0);
			points[1] = new PathVertex(0, size);
			points[2] = new PathVertex(size, 0);
			points[3] = new PathVertex(size, size);

			var indices = new int[6];
			indices[0] = 0; indices[1] = 1; indices[2] = 3;
			indices[3] = 1; indices[4] = 2; indices[5] = 3;

			return new Path(points, indices, 1.0f, 1.0f);
		}
	}

	[Serializable]
	public class EnumerableClass
	{
		public int TestInt { get; private set; }
		public bool FinishedNestedScript { get; private set; }

		public IEnumerator Foo()
		{
			yield return 0;
			TestInt = 5;
			yield return 0;
			yield return 0;
		}

		public IEnumerator NestedIEnumerator()
		{
			yield return 0;
			yield return Delay.Updates(1);
			yield return 0;
			TestInt = 7;
		}

		/// <summary>
		/// Nested Script
		/// </summary>
		public IEnumerator NestedScript(Scripts engine)
		{
			yield return 0;
			yield return engine.Start(Delay.Updates(3), "");
			FinishedNestedScript = true;
		}

		public bool Yielded { get; private set; }

		public IEnumerator YieldOnce()
		{
			Yielded = true;
			yield return 1;
		}
	}
}
