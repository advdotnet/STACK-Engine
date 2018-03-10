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
            Scripts TestScriptEngine = new Scripts();
            TestScriptEngine.Start(new EnumerableClass().Foo());
            Assert.AreEqual(1, TestScriptEngine.ScriptCollection.Count);
        }

        [TestMethod]
        public void ScriptEngineClearScripts()
        {
            Scripts TestScriptEngine = new Scripts();
            TestScriptEngine.Start(new EnumerableClass().Foo());
            TestScriptEngine.Update();
            TestScriptEngine.Clear();
            Assert.AreEqual(0, TestScriptEngine.ScriptCollection.Count);
        }

        [TestMethod]
        public void ExecuteImmediately()
        {
            var TestScriptEngine = new Scripts();
            var Enumerable = new EnumerableClass();
            TestScriptEngine.Start(Enumerable.YieldOnce(), "");
            Assert.IsTrue(Enumerable.Yielded);
            TestScriptEngine.Update();
            Assert.AreEqual(0, TestScriptEngine.ScriptCollection.Count);
        }

        [TestMethod]
        public void ExecuteDeferred()
        {
            var TestScriptEngine = new Scripts();
            var Enumerable = new EnumerableClass();
            TestScriptEngine.Enqueue(Enumerable.YieldOnce(), "");
            Assert.IsFalse(Enumerable.Yielded);
            TestScriptEngine.Update();
            Assert.IsTrue(Enumerable.Yielded);
            TestScriptEngine.Update();
            Assert.AreEqual(0, TestScriptEngine.ScriptCollection.Count);
        }

        [TestMethod]
        public void ScriptEngineNestedScript()
        {
            Scripts TestScriptEngine = new Scripts();
            var EnumerableClass = new EnumerableClass();
            TestScriptEngine.Start(EnumerableClass.NestedScript(TestScriptEngine), "");
            Assert.AreEqual(1, TestScriptEngine.ScriptCollection.Count);
            TestScriptEngine.Update();
            Assert.AreEqual(2, TestScriptEngine.ScriptCollection.Count);
            TestScriptEngine.Update();
            Assert.IsFalse(EnumerableClass.FinishedNestedScript);
            TestScriptEngine.Update();
            Assert.AreEqual(0, TestScriptEngine.ScriptCollection.Count);
            Assert.IsTrue(EnumerableClass.FinishedNestedScript);
        }

        [TestMethod]
        public void ClearSetsScriptDone()
        {
            var TestScriptEngine = new Scripts();
            var Enumerable = new EnumerableClass();
            var Script = TestScriptEngine.Start(Enumerable.Foo(), "ScriptName");
            Assert.IsFalse(Script.Done);
            TestScriptEngine.Update();
            Assert.IsFalse(Script.Done);
            TestScriptEngine.Clear();
            Assert.IsTrue(Script.Done);
        }

        [TestMethod]
        public void NestedIEnumerator()
        {
            var Script = new Scripts();
            var ScriptClass = new EnumerableClass();
            Script.Start(ScriptClass.NestedIEnumerator());
            while (Script.ScriptCollection.Count > 0)
            {
                Script.Update();
            }
            Assert.AreEqual(7, ScriptClass.TestInt);
        }

        /// <summary>
        /// Checks that for a high walking speed the goto script does not get stuck in a loop.
        /// </summary>
        [TestMethod]
        public void HighWalkSpeedLoop()
        {
            var Entity = new Entity();

            Transform.Create(Entity).SetSpeed(800);
            Scripts.Create(Entity);
            Navigation.Create(Entity).SetPath(CreateRectangularPath(100));

            Entity.Get<Scripts>().GoTo(50, 50);

            var LastPosition = new Vector2();
            var i = 0;

            do
            {
                LastPosition = Entity.Get<Transform>().Position;
                Entity.Update();

                if (++i > 10) throw new Exception("Walking for too many updates");

            } while (LastPosition != Entity.Get<Transform>().Position);
        }


        public static Path CreateRectangularPath(int size)
        {
            var Points = new PathVertex[4];

            Points[0] = new PathVertex(0, 0);
            Points[1] = new PathVertex(0, size);
            Points[2] = new PathVertex(size, 0);
            Points[3] = new PathVertex(size, size);

            var Indices = new int[6];
            Indices[0] = 0; Indices[1] = 1; Indices[2] = 3;
            Indices[3] = 1; Indices[4] = 2; Indices[5] = 3;

            return new Path(Points, Indices, 1.0f, 1.0f);
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
