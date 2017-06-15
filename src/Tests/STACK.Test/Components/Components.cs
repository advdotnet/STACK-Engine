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
    public class ComponentsTest
    {

        [TestMethod]
        public void AddComponent()
        {
            Entity Test = new Entity();
            Test.Add<Transform>();
            Assert.IsNotNull(Test.Get<Transform>());            
        }

        [TestMethod]
        public void AddComponentSetsEntity()
        {
            Entity Test = new Entity();
            var Transform = Test.Add<Transform>();
            Assert.AreEqual(Test, Transform.Entity);            
        }

        [TestMethod]
        public void RemoveComponent()
        {
            Entity Test = new Entity();
            Assert.IsNull(Test.Get<Transform>());
            Test.Add<Transform>();
            Assert.AreEqual(1, Test.Items.Count);
            Test.Remove<Transform>();
            Assert.IsNull(Test.Get<Transform>());
            Assert.AreEqual(0, Test.Items.Count);
        }

        [TestMethod]
        public void ModifyComponent()
        {
            Entity Test = new Entity();
            Test.Add<Transform>();
            Test.Get<Transform>().Z = 7;
            Assert.AreEqual(7, Test.Get<Transform>().Z);            
        }

        [TestMethod]
        public void RectangleHotspotAsHotspot()
        {
            Entity Test = new Entity();
            Test.Add<HotspotRectangle>();

            Assert.IsNotNull(Test.Get<Hotspot>());
        }

        [TestMethod]
        public void NotifyDoesNotThrowWithoutEntity()
        {
            new Transform().Position = new Vector2(1, 1);
        }    
    
        [TestMethod]
        public void NoNotificationBeforeInitialization()
        {
            Entity Test = new Entity();
            Test.Add<SpriteData>();            
            Test.Notify(Messages.ColorChanged, Color.Aqua);

            Assert.AreNotEqual(Color.Aqua, Test.Get<SpriteData>().Color);
        }

        [TestMethod]
        public void NotificationAfterInitialization()
        {
            Entity Test = new Entity();
            Test.Add<SpriteData>();

            Test.Initialize();

            Test.Notify(Messages.ColorChanged, Color.Aqua);

            Assert.AreEqual(Color.Aqua, Test.Get<SpriteData>().Color);
        }
    }
}
