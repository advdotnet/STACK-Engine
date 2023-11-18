using Microsoft.VisualStudio.TestTools.UnitTesting;
using STACK.Components;

namespace STACK.Test.Room1
{
	public class MyObj : Entity { }
	public class MyExit : Entity
	{
		public MyExit()
		{
			Add<Exit>();
		}
	}
}

namespace STACK.Test
{
	[TestClass]
	public class GameObjectTest
	{
		[TestMethod]
		public void GameObjectSetsNameSpaceAsID()
		{
			var gameObject = new Room1.MyObj();
			Assert.AreEqual("STACK.Test.Room1.MyObj", gameObject.ID);
		}

		[TestMethod]
		public void ExitSetsNameSpaceAsID()
		{
			var exit = new Room1.MyExit();
			Assert.AreEqual("STACK.Test.Room1.MyExit", exit.ID);
		}

		[TestMethod]
		public void KeepsManualID()
		{
			var gameObject = new Entity("myID");
			Assert.AreEqual("myID", gameObject.ID);
		}
	}
}
