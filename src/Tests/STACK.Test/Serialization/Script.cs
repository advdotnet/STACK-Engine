using System;
using System.Collections.Generic;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using STACK.Components;

namespace STACK.Test
{


    [TestClass]
    public class SaveScriptState
    {
        [TestMethod]
        public void DelayScript()
        {

        }
        
        [TestMethod]
        public void SaveScript()
        {
            var TestClass = new EnumerableClass();
            Script TestScript = new Script(TestClass.Foo());
            TestScript.MoveNext();
            TestScript.MoveNext();
            Assert.AreEqual(false, TestScript.Done);
            byte[] Check = STACK.State.State.SaveState<Script>(TestScript);


            TestScript = STACK.State.State.LoadState<Script>(Check);
            Assert.AreEqual(false, TestScript.Done);            
        }

        [TestMethod]
        public void ScriptEngineSerialize()
        {
            Scripts TestScriptEngine = new Scripts();
            TestScriptEngine.Start(new EnumerableClass().Foo());
            TestScriptEngine.Update();

            byte[] Check = STACK.State.State.SaveState<Scripts>(TestScriptEngine);
            TestScriptEngine.Update();

            TestScriptEngine = STACK.State.State.LoadState<Scripts>(Check);
            TestScriptEngine.Update();
        }               
    }
}
