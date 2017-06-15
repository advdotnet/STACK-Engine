using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using STACK;
using STACK.Input;
using STACK.Components;
using StarFinder;

namespace STACK.Test
{
    [TestClass]
    public class NavigationTest
    {           
        [TestMethod]
        public void UseScenePathEnterSceneTest()
        {
            Entity Entity = new Entity();
            Navigation.Create(Entity).SetUseScenePath(true);
            Scene Scene = new Scene("s1");
            ScenePath.Create(Scene).SetPath(ScriptsTest.CreateRectangularPath(100));
            Scene.Push(Entity);
            Scene.Initialize();
            Entity.EnterScene(Scene);
            Assert.AreEqual(Scene.Get<ScenePath>().Path, Entity.Get<Navigation>().Path);
        }

        [TestMethod]
        public void DoNotUseScenePathEnterSceneTest()
        {
            Entity Entity = new Entity();
            Navigation.Create(Entity).SetUseScenePath(false);
            Scene Scene = new Scene("s1");
            ScenePath.Create(Scene).SetPath(ScriptsTest.CreateRectangularPath(100));
            Scene.Push(Entity);
            Entity.EnterScene(Scene);
            Assert.AreEqual(null, Entity.Get<Navigation>().Path);
        }

        [TestMethod]
        public void UseScenePathScenePathChangesTest()
        {
            Entity Entity = new Entity();
            Navigation.Create(Entity).SetUseScenePath(true);
            Scene Scene = new Scene("s1");
            ScenePath.Create(Scene).SetPath(ScriptsTest.CreateRectangularPath(100));
            Scene.Push(Entity);
            Scene.Initialize();
            Scene.Get<ScenePath>().Path = ScriptsTest.CreateRectangularPath(50);           
            Assert.AreEqual(Scene.Get<ScenePath>().Path, Entity.Get<Navigation>().Path);
        }

        [TestMethod]
        public void RestrictPositionTest()
        {
            Entity Entity = new Entity();
            Navigation.Create(Entity).SetUseScenePath(true).SetRestrictPosition(true);
            Transform.Create(Entity).SetPosition(200, 200);
            Scene Scene = new Scene("s1");
            ScenePath.Create(Scene);
            Scene.Push(Entity);
            Scene.Initialize();
            var Path = ScriptsTest.CreateRectangularPath(50);
            Assert.IsFalse(Path.Contains(Entity.Get<Transform>().Position));
            Scene.Get<ScenePath>().Path = Path;
            Assert.IsTrue(Path.Contains(Entity.Get<Transform>().Position));
        }


        
    }
}
