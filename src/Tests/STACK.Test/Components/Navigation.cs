using Microsoft.VisualStudio.TestTools.UnitTesting;
using STACK.Components;

namespace STACK.Test
{
	[TestClass]
	public class NavigationTest
	{
		[TestMethod]
		public void UseScenePathEnterSceneTest()
		{
			var entity = new Entity();
			Navigation.Create(entity).SetUseScenePath(true);
			var scene = new Scene("s1");
			ScenePath.Create(scene).SetPath(ScriptsTest.CreateRectangularPath(100));
			scene.Push(entity);
			scene.Initialize(false);
			entity.EnterScene(scene);
			Assert.AreEqual(scene.Get<ScenePath>().Path, entity.Get<Navigation>().Path);
		}

		[TestMethod]
		public void DoNotUseScenePathEnterSceneTest()
		{
			var entity = new Entity();
			Navigation.Create(entity).SetUseScenePath(false);
			var scene = new Scene("s1");
			ScenePath.Create(scene).SetPath(ScriptsTest.CreateRectangularPath(100));
			scene.Push(entity);
			entity.EnterScene(scene);
			Assert.AreEqual(null, entity.Get<Navigation>().Path);
		}

		[TestMethod]
		public void UseScenePathScenePathChangesTest()
		{
			var entity = new Entity();
			Navigation.Create(entity).SetUseScenePath(true);
			var scene = new Scene("s1");
			ScenePath.Create(scene).SetPath(ScriptsTest.CreateRectangularPath(100));
			scene.Push(entity);
			scene.Initialize(false);
			scene.Get<ScenePath>().Path = ScriptsTest.CreateRectangularPath(50);
			Assert.AreEqual(scene.Get<ScenePath>().Path, entity.Get<Navigation>().Path);
		}

		[TestMethod]
		public void RestrictPositionTest()
		{
			var entity = new Entity();
			Navigation.Create(entity).SetUseScenePath(true).SetRestrictPosition(true);
			Transform.Create(entity).SetPosition(200, 200);
			var scene = new Scene("s1");
			ScenePath.Create(scene);
			scene.Push(entity);
			scene.Initialize(false);
			var path = ScriptsTest.CreateRectangularPath(50);
			Assert.IsFalse(path.Contains(entity.Get<Transform>().Position));
			scene.Get<ScenePath>().Path = path;
			Assert.IsTrue(path.Contains(entity.Get<Transform>().Position));
		}



	}
}
