using Microsoft.VisualStudio.TestTools.UnitTesting;
using STACK.Components;

namespace STACK.Test
{


    [TestClass]
    public class SaveScriptState
    {
        [TestMethod]
        public void DelayScript()
        {
            var TestScript = new Script(Delay.Updates(2));
            Assert.IsFalse(TestScript.Done);
            TestScript.MoveNext();
            Assert.IsFalse(TestScript.Done);
            TestScript.MoveNext();
            Assert.IsTrue(TestScript.Done);
        }

        [TestMethod]
        public void SaveScript()
        {
            var TestClass = new EnumerableClass();
            var TestScript = new Script(TestClass.Foo());
            TestScript.MoveNext();
            TestScript.MoveNext();
            Assert.AreEqual(false, TestScript.Done);
            var Check = STACK.State.Serialization.SaveState<Script>(TestScript);
            TestScript = STACK.State.Serialization.LoadState<Script>(Check);
            Assert.AreEqual(false, TestScript.Done);
        }

        [TestMethod]
        public void ScriptEngineSerialize()
        {
            var TestScriptEngine = new Scripts();
            TestScriptEngine.Start(new EnumerableClass().Foo());
            TestScriptEngine.Update();

            var Check = STACK.State.Serialization.SaveState<Scripts>(TestScriptEngine);
            TestScriptEngine.Update();

            TestScriptEngine = STACK.State.Serialization.LoadState<Scripts>(Check);
            TestScriptEngine.Update();
        }
    }
}
