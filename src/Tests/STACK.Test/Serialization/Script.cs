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
			var testScript = new Script(Delay.Updates(2));
			Assert.IsFalse(testScript.Done);
			testScript.MoveNext();
			Assert.IsFalse(testScript.Done);
			testScript.MoveNext();
			Assert.IsTrue(testScript.Done);
		}

		[TestMethod]
		public void SaveScript()
		{
			var testClass = new EnumerableClass();
			var testScript = new Script(testClass.Foo());
			testScript.MoveNext();
			testScript.MoveNext();
			Assert.AreEqual(false, testScript.Done);
			var check = State.Serialization.SaveState(testScript);
			testScript = State.Serialization.LoadState<Script>(check);
			Assert.AreEqual(false, testScript.Done);
		}

		[TestMethod]
		public void ScriptEngineSerialize()
		{
			var testScriptEngine = new Scripts();
			testScriptEngine.Start(new EnumerableClass().Foo());
			testScriptEngine.Update();

			var check = State.Serialization.SaveState(testScriptEngine);
			testScriptEngine.Update();

			testScriptEngine = State.Serialization.LoadState<Scripts>(check);
			testScriptEngine.Update();
		}
	}
}
